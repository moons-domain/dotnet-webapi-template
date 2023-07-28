
namespace RichWebApi.Persistence.Internal;

internal interface IEntityValidatorsProvider
{
	EntityAsyncValidator GetAsyncValidator(IServiceProvider serviceProvider, Type entityType);
	
	public IReadOnlyDictionary<Type, (Func<IServiceProvider, object> ValidatorProvider, AsyncValidationExecutor
		ValidationExecutor
		)> AsyncValidators { get; }
	bool AllEntitiesHaveValidators { get; }
}