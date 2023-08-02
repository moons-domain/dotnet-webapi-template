using Polly;

namespace RichWebApi.Utilities;

public interface IDatabasePolicySet
{
	IAsyncPolicy DatabaseReadPolicy { get; }
	IAsyncPolicy DatabaseWritePolicy { get; }
}