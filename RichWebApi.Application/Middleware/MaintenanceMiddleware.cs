using System.Text.Json;
using RichWebApi.Maintenance;

namespace RichWebApi.Middleware;

public class MaintenanceMiddleware
{
	private readonly RequestDelegate _next;

	public MaintenanceMiddleware(RequestDelegate next) => _next = next;

	public async Task Invoke(HttpContext context)
	{
		var maintenance = context.RequestServices.GetRequiredService<ApplicationMaintenance>();
		if (maintenance.IsEnabled)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
			await context.Response.WriteAsync(JsonSerializer.Serialize(maintenance.Info));
			return;
		}

		await _next(context);
	}
}