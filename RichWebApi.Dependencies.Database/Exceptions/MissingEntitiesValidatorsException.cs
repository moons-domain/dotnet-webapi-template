namespace RichWebApi.Exceptions;

public class MissingEntitiesValidatorsException : DatabaseDependencyException
{
	public IReadOnlyCollection<Type> MissingTypes { get; }

	public MissingEntitiesValidatorsException(IReadOnlyCollection<Type> missingTypes) : base(FormatMissingTypesMessage(missingTypes)) 
		=> MissingTypes = missingTypes;

	private static string FormatMissingTypesMessage(IEnumerable<Type> missingTypes)
	{
		var header = $"Some of entities have no validators:{Environment.NewLine}";
		return $"{header}{string.Join(Environment.NewLine, missingTypes.Select(x => x.Name))}";
	}
}