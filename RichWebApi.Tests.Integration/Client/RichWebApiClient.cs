using Newtonsoft.Json;

namespace RichWebApi.Tests.Client;

internal abstract class RichWebApiClient : IRichWebApiClient
{
	protected virtual ValueTask<HttpRequestMessage> CreateHttpRequestMessageAsync(
		CancellationToken cancellationToken = default)
		=> new(new HttpRequestMessage());

	public static void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
	{

	}
}