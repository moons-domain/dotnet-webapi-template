using Newtonsoft.Json;

namespace RichWebApi.Tests.Client;

public interface IRichWebApiClient
{
	void UpdateJsonSerializerSettings(JsonSerializerSettings settings);
}