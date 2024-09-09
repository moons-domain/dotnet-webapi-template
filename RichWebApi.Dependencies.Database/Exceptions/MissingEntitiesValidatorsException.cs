namespace RichWebApi.Exceptions;

public class MissingEntitiesValidatorsException(IReadOnlyCollection<Type> missingTypes)
	: DatabaseDependencyException(FormatMissingTypesMessage(missingTypes))
{
	public IReadOnlyCollection<Type> MissingTypes { get; } = missingTypes;

	private static string FormatMissingTypesMessage(IEnumerable<Type> missingTypes)
	{
		var header = $"Some of entities have no validators:{Environment.NewLine}";
		return $"{header}{string.Join(Environment.NewLine, missingTypes.Select(x => x.Name))}";
	}
}