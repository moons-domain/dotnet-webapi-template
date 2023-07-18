using Polly;

namespace RichWebApi.Utilities;

internal interface IDatabasePolicySet
{
	IAsyncPolicy DatabaseReadPolicy { get; }
	IAsyncPolicy DatabaseWritePolicy { get; }
}