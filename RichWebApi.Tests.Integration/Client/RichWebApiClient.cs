using Newtonsoft.Json;

namespace RichWebApi.Tests.Client;

internal abstract class RichWebApiClient : IRichWebApiClient
{
	public virtual void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
	{

	}
}