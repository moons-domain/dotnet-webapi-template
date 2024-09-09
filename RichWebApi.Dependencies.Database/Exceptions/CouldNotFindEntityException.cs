using System.Reflection;

namespace RichWebApi.Exceptions;

public class CouldNotFindEntityException : DatabaseDependencyException
{

	public CouldNotFindEntityException(MemberInfo entity) : base($"Couldn't find related {entity.Name.ToLowerInvariant()}")
	{
	}

	public CouldNotFindEntityException(string entity) : base(
		$"Couldn't find related {entity}")
	{
	}
}