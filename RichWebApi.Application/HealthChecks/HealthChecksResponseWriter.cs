using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RichWebApi.HealthChecks;

public static class HealthChecksResponseWriter
{
	private static readonly JsonSerializerOptions jsonOptions = new()
	{
		WriteIndented = false,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		Converters =
		{
			new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
		}
	};

	public static Task WriteAsync(HttpContext context, HealthReport report)
	{
		var json = JsonSerializer.Serialize(report, jsonOptions);
		context.Response.ContentType = MediaTypeNames.Application.Json;
		return context.Response.WriteAsync(json);
	}
}