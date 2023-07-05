namespace RichWebApi.Exceptions;

public abstract class RichWebApiException : ApplicationException
{
	protected RichWebApiException(string message) : base(message)
	{
	}

	public virtual int StatusCode { get; } = 500;
}