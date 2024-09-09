namespace RichWebApi.Exceptions;

public abstract class RichWebApiException : ApplicationException
{
	protected RichWebApiException(string message) : base(message)
	{
	}

	protected RichWebApiException(string? message, Exception? innerException) : base(message, innerException)
	{
	}

	public virtual int StatusCode { get; } = 500;
}