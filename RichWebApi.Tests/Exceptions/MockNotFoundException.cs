using System.Runtime.Serialization;

namespace RichWebApi.Tests.Exceptions;

[Serializable]
public class MockNotFoundException : TestConfigurationException
{
	public MockNotFoundException(Type type) : base($"Mock for type {type.Name} was not found.")
	{
	}

	protected MockNotFoundException(
		SerializationInfo info,
		StreamingContext context) : base(info, context)
	{
	}
}