
namespace RichWebApi.Tests.Exceptions;

[Serializable]
public class TestDataException(string? message) : TestConfigurationException(message);