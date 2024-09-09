namespace RichWebApi.Tests.Exceptions;

[Serializable]
public class TestConfigurationException : RichWebApiTestException
{
	public TestConfigurationException()
	{
	}

	public TestConfigurationException(string? message) : base(message)
	{
	}
}