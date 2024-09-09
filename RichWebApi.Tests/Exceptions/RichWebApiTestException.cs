using System.Runtime.Serialization;

namespace RichWebApi.Tests.Exceptions;

public class RichWebApiTestException : Exception
{
	public RichWebApiTestException()
	{
	}

	public RichWebApiTestException(string? message) : base(message)
	{
	}

	public RichWebApiTestException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}