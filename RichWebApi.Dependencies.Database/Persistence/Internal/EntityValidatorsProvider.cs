using System.Diagnostics;
using FluentValidation;
using FluentValidation.Results;
using static System.Linq.Expressions.Expression;


namespace RichWebApi.Persistence.Internal;

internal class EntityValidatorsProvider : IEntityValidatorsProvider
{
	private delegate Task<ValidationResult> AsyncValidationExecutor(object validator,
																	object entity,
																	CancellationToken cancellationToken);


	private readonly IDictionary<Type, AsyncValidationExecutor> _asyncValidators =
		new Dictionary<Type, AsyncValidationExecutor>();

	private readonly IDictionary<Type, Func<IServiceProvider, object?>> _validatorGetters =
		new Dictionary<Type, Func<IServiceProvider, object?>>();


	public EntityAsyncValidator? GetAsyncValidator(IServiceProvider serviceProvider, Type entityType)
	{
		var entityValidator = GetOrCreateEntityValidator(serviceProvider, entityType);

		if (entityValidator is null)
		{
			return null;
		}

		var (validator, validate) = entityValidator.Value;
		return (entity, token) => validate(validator, entity, token);
	}

	private (object, AsyncValidationExecutor)? GetOrCreateEntityValidator(
		IServiceProvider serviceProvider, Type entityType)
	{
		if (_validatorGetters.TryGetValue(entityType, out var getter)
			&& _asyncValidators.TryGetValue(entityType, out var validate))
		{
			return (getter(serviceProvider), validate)!;
		}

		var validatorType = typeof(IValidator<>).MakeGenericType(entityType);

		object? ProvideValidator(IServiceProvider sp) => sp.GetService(validatorType);
		var validator = ProvideValidator(serviceProvider);

		if (validator == null)
		{
			return null;
		}


		var validatorParameterExpr = Parameter(typeof(object), "validator");
		var method = validatorType.GetMethod(nameof(IValidator<object>.ValidateAsync),
			System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

		Debug.Assert(method != null && method.ReturnType == typeof(Task<ValidationResult>));

		var entityParameterExpr = Parameter(typeof(object), "entity");

		var ctParameterExpr = Parameter(typeof(CancellationToken), "cancellationToken");
		var validateFunc = Lambda<AsyncValidationExecutor>(
			Call(Convert(validatorParameterExpr, validatorType), method, Convert(entityParameterExpr, entityType), ctParameterExpr), validatorParameterExpr,
			entityParameterExpr, ctParameterExpr
		).Compile();

		_validatorGetters[entityType] = ProvideValidator;
		_asyncValidators[entityType] = validateFunc;

		return (validator, validateFunc);
	}
}