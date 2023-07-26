using System.Runtime.Serialization;

namespace RichWebApi.Tests.Core.Exceptions;

[Serializable]
public class TestConfigurationException : RichWebApiTestException
{
    public TestConfigurationException()
    {
    }

    public TestConfigurationException(string? message) : base(message)
    {
    }

    protected TestConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}