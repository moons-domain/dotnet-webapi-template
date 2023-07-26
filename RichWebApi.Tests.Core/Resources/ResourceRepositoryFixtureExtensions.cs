using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using RichWebApi.Tests.Core.Exceptions;

namespace RichWebApi.Tests.Core.Resources;

public static class ResourceRepositoryFixtureExtensions
{
	public static T GetJsonInputResource<T>(this IResourceScope resourceScope, string suffix = "input",
	                                        TestJsonSerializerSettings? jsonSettings = null,
	                                        [CallerMemberName] string? callerMemberName = null)
		=> GetJsonResource<T>(resourceScope, suffix, callerMemberName, jsonSettings)
		   ?? throw new TestDataException("Failed to deserialize JSON object");

	private static T? GetJsonResource<T>(IResourceScope resourceScope, string suffix, string? name,
	                                     TestJsonSerializerSettings? jsonSettings = null)
	{
		var stream = resourceScope.GetResourceStream($"{name}.{suffix}");
		using var sr = new StreamReader(stream);
		using var reader = new JsonTextReader(sr);
		return JsonSerializer
			.Create(jsonSettings ?? new TestJsonSerializerSettings())
			.Deserialize<T>(reader);
	}
}