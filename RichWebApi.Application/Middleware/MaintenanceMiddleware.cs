using RichWebApi.Maintenance;

namespace RichWebApi.Middleware;

public class MaintenanceMiddleware
{
	private readonly RequestDelegate _next;

	public MaintenanceMiddleware(RequestDelegate next) => _next = next;

	public async Task Invoke(HttpContext context)
	{
		var maintenance = context.RequestServices.GetRequiredService<IApplicationMaintenance>();
		if (maintenance.IsEnabled)
		{
			context.Response.ContentType = "text/plain";
			context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
			await context.Response.WriteAsync($"Maintenance mode is enabled. Reason: {maintenance.Reason}");
			return;
		}

		await _next(context);
	}
}