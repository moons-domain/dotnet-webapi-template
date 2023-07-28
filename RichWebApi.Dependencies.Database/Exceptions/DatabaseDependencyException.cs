namespace RichWebApi.Exceptions;

public abstract class DatabaseDependencyException : DependencyException
{
	protected DatabaseDependencyException(string message) : base(message)
	{
	}
}