using System.Runtime.Serialization;

namespace RichWebApi.Tests.Exceptions;

public class RichWebApiTestException : Exception
{
	public RichWebApiTestException()
	{
	}

	protected RichWebApiTestException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}

	public RichWebApiTestException(string? message) : base(message)
	{
	}

	public RichWebApiTestException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}