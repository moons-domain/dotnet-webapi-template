using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using RichWebApi;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
	// When something wrong with logging - uncomment the line below
	// Serilog.Debugging.SelfLog.Enable(Console.Error);

	var logOutputTemplate = "[{Timestamp:HH:mm:ss.fff}] "
							+ "[{RequestId}] "
							+ "[{SourceContext:l}] "
							+ "[{Level:u3}] "
							+ "{Message:lj}{NewLine}{Exception}";

	loggerConfiguration
		.ReadFrom.Configuration(context.Configuration)
		.Enrich.FromLogContext()
		.Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
		.Enrich.WithThreadId();

	if (context.HostingEnvironment.IsDevelopment())
	{
		loggerConfiguration.WriteTo.Console(
			outputTemplate: logOutputTemplate,
			theme: AnsiConsoleTheme.Literate,
			restrictedToMinimumLevel: LogEventLevel.Debug);
	}
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
	s.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "RichWebApi",
		Version = "v1"
	});
});

builder.Services.AddHealthChecks();
builder.Services
	.AddMvcCore()
	.AddBasePart();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHealthChecks(new PathString("/api/health"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();