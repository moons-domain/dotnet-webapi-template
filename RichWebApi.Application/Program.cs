using AutoMapper.EquivalencyExpression;
using MediatR;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using RichWebApi;
using RichWebApi.Maintenance;
using RichWebApi.MediatR;
using RichWebApi.Middleware;
using RichWebApi.Parts;
using RichWebApi.Startup;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
	.AddEnvironmentVariables()
	.AddUserSecrets<Program>(true, true);

builder.Host.UseSerilog((context, sp, loggerConfiguration) =>
{
	// When something wrong with logging - uncomment the line below
	// Serilog.Debugging.SelfLog.Enable(Console.Error);

	const string logOutputTemplate = "[{Timestamp:HH:mm:ss.fff}] "
									 + "[{RequestId}] "
									 + "[{SourceContext:l}] "
									 + "[{Level:u3}] "
									 + "{Message:lj}{NewLine}"
									 + "{Properties:j}{NewLine}"
									 + "{Exception}";

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
				restrictedToMinimumLevel: LogEventLevel.Debug)
			.WriteTo.Seq("http://localhost:5341");
	}
});

var applicationDependencies = new AppDependenciesCollection()
	.AddDatabase(builder.Environment)
	.AddSignalR(_ => { });

var applicationParts = new AppPartsCollection()
	.AddWeather();

// Add services to the container.
var services = builder.Services;

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(s =>
{
	s.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "RichWebApi",
		Version = "v1"
	});
	s.AddSignalRSwaggerGen();
});

services.AddFluentValidationRulesToSwagger(opt => opt.SetFluentValidationCompatibility());



services
	.AddCore();

services.AddHealthChecks();

services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceLoggingBehavior<,>));


services.AddAutoMapper(x => x.AddCollectionMappers(), typeof(Program).Assembly);
services.EnrichWithApplicationParts(applicationParts);
services.AddDependencyServices(applicationDependencies);

services.AddStartupAction<AutoMapperValidationAction>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHealthChecks(new PathString("/api/health"));
app.UseMiddleware<MaintenanceMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();
app.UseRouting();

app.UseAuthorization();
app.UseDependencies(applicationDependencies);

var lifetime = app.Lifetime;

try
{
	var appRunner = app.RunAsync();
	lifetime.ApplicationStarted.WaitHandle.WaitOne();

	await using (var scope = app.Services.CreateAsyncScope())
	{
		var sp = scope.ServiceProvider;
		var maintenance = sp.GetRequiredService<IApplicationMaintenance>();
		maintenance.Enable("startup");
		await sp.GetRequiredService<IStartupActionCoordinator>()
			.PerformStartupActionsAsync(lifetime.ApplicationStopping);
		maintenance.Disable();
	}

	await appRunner;
}
catch (Exception e)
{
	Log.Error(e, "Application exited with error");
}
finally
{
	await Log.CloseAndFlushAsync();
}