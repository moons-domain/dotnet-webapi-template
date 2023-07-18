
namespace RichWebApi.Persistence.Internal;

internal interface IEntityValidatorsProvider
{
	EntityAsyncValidator? GetAsyncValidator(IServiceProvider serviceProvider, Type entityType);
	bool AllEntitiesHaveValidators { get; }
}