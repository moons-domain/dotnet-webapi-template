using Newtonsoft.Json;

namespace RichWebApi.Tests.Client;

internal abstract class RichWebApiClient : IRichWebApiClient
{
	protected void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
	{

	}
}