using System.Runtime.Serialization;

namespace RichWebApi.Tests.Exceptions;

[Serializable]
public class TestDataException : TestConfigurationException
{
    public TestDataException(string? message) : base(message)
    {
    }

    protected TestDataException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}