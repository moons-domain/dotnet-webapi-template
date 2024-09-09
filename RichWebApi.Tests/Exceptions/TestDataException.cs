
namespace RichWebApi.Tests.Exceptions;

[Serializable]
public class TestDataException : TestConfigurationException
{
	public TestDataException(string? message) : base(message)
	{
	}
}