using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RichWebApi.Config;
using RichWebApi.Entities;
using RichWebApi.Exceptions;
using RichWebApi.Extensions;
using static System.Linq.Expressions.Expression;

namespace RichWebApi.Persistence.Internal;

internal class EntityValidatorsProvider : IEntityValidatorsProvider
{
	private readonly ILogger<EntityValidatorsProvider> _logger;

	public IReadOnlyDictionary<Type, (Func<IServiceProvider, object> ValidatorProvider, AsyncValidationExecutor
		ValidationExecutor
		)> AsyncValidators
	{ get; }

	public bool AllEntitiesHaveValidators { get; }

	public EntityValidatorsProvider(ILogger<EntityValidatorsProvider> logger,
									IServiceProvider serviceProvider,
									IOptionsMonitor<DatabaseEntitiesConfig> configMonitor,
									IEnumerable<IAppPart> partsToScan)
	{
		_logger = logger;
		var entityType = typeof(IEntity);
		var entityTypes = partsToScan
			.SelectMany(x => x.GetType().Assembly.ExportedTypes
				.Where(t => t is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false }
							&& t.IsAssignableTo(entityType)))
			.ToArray();
		var validators = logger.Time(() =>
		{
			using var scope = serviceProvider.CreateScope();
			var sp = scope.ServiceProvider;
			return entityTypes.Select(x => (Type: x, MaybeValidator: CreateEntityValidator(sp, x)))
				.Where(x => x.MaybeValidator.HasValue)
				.ToDictionary(x => x.Type, x => x.MaybeValidator!.Value);
		}, "Build validator callers for entities, count: {Count}", entityTypes.Length);
		AllEntitiesHaveValidators = entityTypes.Length == validators.Count;
		AsyncValidators = validators.AsReadOnly();

		if (configMonitor.CurrentValue.Validation == EntitiesValidationOption.Required && !AllEntitiesHaveValidators)
		{
			var missingValidatorTypes = entityTypes
				.Where(x => !validators.ContainsKey(x))
				.ToArray();
			throw new MissingEntitiesValidatorsException(missingValidatorTypes);
		}
	}

	public EntityAsyncValidator GetAsyncValidator(IServiceProvider serviceProvider, Type entityType)
	{
		if (!AsyncValidators.TryGetValue(entityType, out var entityValidator))
		{
			throw new MissingEntitiesValidatorsException(new[] { entityType });
		}

		var (validatorProvider, validate) = entityValidator;
		var validator = validatorProvider(serviceProvider);
		return (entity, token) => validate(validator, entity, token);
	}

	private (Func<IServiceProvider, object>, AsyncValidationExecutor)? CreateEntityValidator(
		IServiceProvider serviceProvider, Type entityType)
	{
		var entityName = entityType.Name;


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

			if (method is null)
			{
				throw new InvalidOperationException(
					$"Type '{validatorType.Name}' doesn't contain method '{nameof(IValidator<object>.ValidateAsync)}' which should be used in validation delegate.");
			}

			var entityParameterExpr = Parameter(typeof(object), "entity");

			var ctParameterExpr = Parameter(typeof(CancellationToken), "cancellationToken");
			return Lambda<AsyncValidationExecutor>(
				Call(Convert(validatorParameterExpr, validatorType), method, Convert(entityParameterExpr, entityType),
					ctParameterExpr), validatorParameterExpr,
				entityParameterExpr, ctParameterExpr
			).Compile();
		}, "Build validator caller for entity '{EntityName}'", entityName);

		return (ProvideValidator!, validateFunc);
	}
}