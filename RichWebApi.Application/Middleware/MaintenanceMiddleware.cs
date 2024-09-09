using System.Net.Mime;
using System.Text.Json;
using RichWebApi.Maintenance;

namespace RichWebApi.Middleware;

public class MaintenanceMiddleware(RequestDelegate next)
{
	public async Task Invoke(HttpContext context)
	{
		var maintenance = context.RequestServices.GetRequiredService<ApplicationMaintenance>();
		if (maintenance.IsEnabled)
		{
			context.Response.ContentType = MediaTypeNames.Application.Json;
			context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
			await context.Response.WriteAsync(JsonSerializer.Serialize(maintenance.Info));
			return;
		}

		await next(context);
	}
}