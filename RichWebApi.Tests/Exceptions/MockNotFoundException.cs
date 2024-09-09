namespace RichWebApi.Tests.Exceptions;

[Serializable]
public class MockNotFoundException(Type type) : TestConfigurationException($"Mock for type {type.Name} was not found.");