namespace RichWebApi.Exceptions;

public abstract class DependencyException : RichWebApiException
{
	protected DependencyException(string message) : base(message)
	{
	}
}