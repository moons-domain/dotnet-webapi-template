using System.Runtime.CompilerServices;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Newtonsoft.Json;
using RichWebApi.Tests.Core.Exceptions;
using Xunit.Abstractions;

namespace RichWebApi.Tests.Core.Resources;

public static class ResourceScopeExtensions
{
	public static T GetJsonInputResource<T>(this IResourceScope resourceScope, string suffix = "input",
	                                        TestJsonSerializerSettings? jsonSettings = null,
	                                        [CallerMemberName] string? callerMemberName = null)
		=> GetJsonResource<T>(resourceScope, suffix, callerMemberName, jsonSettings)
		   ?? throw new TestDataException("Failed to deserialize JSON object");

	public static Stream GetInputResourceStream(this IResourceScope resourceScope, string suffix = "input",
	                                            [CallerMemberName] string? callerMemberName = null)
		=> resourceScope.GetResourceStream($"{callerMemberName}.{suffix}");

	public static void CompareWithJsonExpectation<T>(this IResourceScope resourceScope,
	                                                 ITestOutputHelper testOutputHelper,
	                                                 T actual,
	                                                 string suffix = "expected",
	                                                 TestJsonSerializerSettings? jsonSettings = null,
	                                                 Func<EquivalencyAssertionOptions<T?>,
		                                                 EquivalencyAssertionOptions<T?>>? config = null,
	                                                 [CallerMemberName] string? caller = null) where T : class
	{
		var definedJsonSettings = jsonSettings ?? new TestJsonSerializerSettings();
		var expectation = GetJsonResource<T>(resourceScope, suffix, caller);
		var actualCopy = actual.JsonCopy(definedJsonSettings);

		try
		{
			actualCopy.Should().BeEquivalentTo(expectation, x =>
			{
				config?.Invoke(x);
				x.RespectingRuntimeTypes();
				return x;
			});
		}
		catch
		{
			testOutputHelper.WriteLine("Actual for {0}.{1} is:\n{2}", caller, suffix, actualCopy.ToJson(definedJsonSettings));
			throw;
		}
	}


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