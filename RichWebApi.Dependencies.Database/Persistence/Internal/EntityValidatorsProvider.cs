using System.Diagnostics;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RichWebApi.Entities;
using RichWebApi.Extensions;
using static System.Linq.Expressions.Expression;


namespace RichWebApi.Persistence.Internal;

internal class EntityValidatorsProvider : IEntityValidatorsProvider
{
	private readonly ILogger<EntityValidatorsProvider> _logger;

	private readonly
		IDictionary<Type, (Func<IServiceProvider, object> ValidatorProvider, AsyncValidationExecutor ValidationExecutor
			)> _asyncValidators =
			new Dictionary<Type, (Func<IServiceProvider, object>, AsyncValidationExecutor)>();

	public bool AllEntitiesHaveValidators { get; }

	public EntityValidatorsProvider(ILogger<EntityValidatorsProvider> logger, IServiceProvider serviceProvider,
									IEnumerable<IAppPart> partsToScan)
	{
		_logger = logger;

		using (logger.BeginScope("entity validator callers build"))
		{
			var entityType = typeof(IEntity);
			var entityTypes = partsToScan
				.SelectMany(x => x.GetType().Assembly.ExportedTypes
					.Where(t => t is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false }
								&& t.IsAssignableTo(entityType)))
				.ToArray();
			var validatorsCount = logger.Time(() =>
			{
				using var scope = serviceProvider.CreateScope();
				var sp = scope.ServiceProvider;
				return entityTypes.Select(x => GetOrCreateEntityValidator(sp, x)).Count();
			}, "Build validator callers for entities, count: {Count}", entityTypes.Length);
			AllEntitiesHaveValidators = entityTypes.Length == validatorsCount;
		}
	}

	private delegate Task<ValidationResult> AsyncValidationExecutor(object validator,
																	object entity,
																	CancellationToken cancellationToken);


	public EntityAsyncValidator? GetAsyncValidator(IServiceProvider serviceProvider, Type entityType)
	{
		var entityValidator = GetOrCreateEntityValidator(serviceProvider, entityType);

		if (entityValidator is null)
		{
			return null;
		}

		var (validatorProvider, validate) = entityValidator.Value;
		var validator = validatorProvider(serviceProvider);
		return (entity, token) => validate(validator, entity, token);
	}

	private (Func<IServiceProvider, object>, AsyncValidationExecutor)? GetOrCreateEntityValidator(
		IServiceProvider serviceProvider, Type entityType)
	{
		var entityName = entityType.Name;

		if (_asyncValidators.TryGetValue(entityType, out var fromCache))
		{
			_logger.LogDebug("Got entity '{EntityName}' validator caller from cache", entityName);
			return fromCache;
		}

		var validatorType = typeof(IValidator<>).MakeGenericType(entityType);

		object? ProvideValidator(IServiceProvider sp) => sp.GetService(validatorType);
		var validator = ProvideValidator(serviceProvider);

		if (validator == null)
		{
			_logger.LogDebug("Missing validator for entity '{EntityName}'", entityName);
			return null;
		}

		var validateFunc = _logger.Time(() =>
		{
			var validatorParameterExpr = Parameter(typeof(object), "validator");
			var method = validatorType.GetMethod(nameof(IValidator<object>.ValidateAsync),
				System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

			Debug.Assert(method != null && method.ReturnType == typeof(Task<ValidationResult>));

			var entityParameterExpr = Parameter(typeof(object), "entity");

			var ctParameterExpr = Parameter(typeof(CancellationToken), "cancellationToken");
			return Lambda<AsyncValidationExecutor>(
				Call(Convert(validatorParameterExpr, validatorType), method, Convert(entityParameterExpr, entityType),
					ctParameterExpr), validatorParameterExpr,
				entityParameterExpr, ctParameterExpr
			).Compile();
		}, "Build validator caller for entity '{EntityName}'", entityName);

		return _asyncValidators[entityType] = (ProvideValidator!, validateFunc);
	}
}