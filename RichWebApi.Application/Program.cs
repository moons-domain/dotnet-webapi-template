using AutoMapper.EquivalencyExpression;
using MediatR;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using RichWebApi.Maintenance;
using RichWebApi.MediatR;
using RichWebApi.Middleware;
using RichWebApi.Startup;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace RichWebApi;

public class Program
{
	public static Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		ConfigureConfiguration(builder.Configuration);
		ConfigureHost(builder.Host);

		var dependencies = EnrichWithDependencies(new AppDependenciesCollection(), builder.Environment);
		var parts = EnrichWithApplicationParts(new AppPartsCollection());

		ConfigureServices(builder.Services, parts, dependencies);

		var app = Configure(builder.Build(), dependencies);
		return RunAsync(app);
	}

	private static async Task RunAsync(WebApplication app)
	{
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
	}

	private static IConfigurationBuilder ConfigureConfiguration(IConfigurationBuilder configuration)
		=> configuration
			.AddEnvironmentVariables()
			.AddUserSecrets<Program>(true, true);

	private static IHostBuilder ConfigureHost(IHostBuilder host)
		=> host.UseSerilog((context, sp, loggerConfiguration) =>
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

	private static IAppDependenciesCollection EnrichWithDependencies(IAppDependenciesCollection collection,
																	 IWebHostEnvironment env)
		=> collection.AddDatabase(env)
			.AddSignalR(c => c.AddWeather());

	public static IAppPartsCollection EnrichWithApplicationParts(IAppPartsCollection collection)
		=> collection.AddWeather();

	private static IServiceCollection ConfigureServices(IServiceCollection services, IAppPartsCollection parts,
														IAppDependenciesCollection dependencies)
	{
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

		services.AddCore();

		services.AddHealthChecks();

		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceLoggingBehavior<,>));


		services.AddAutoMapper(x => x.AddCollectionMappers(), typeof(Program).Assembly);
		services.AddAppParts(parts);
		services.AddDependencyServices(dependencies, parts);

		services.AddStartupAction<AutoMapperValidationAction>();
		return services;
	}

	private static WebApplication Configure(WebApplication app, IAppDependenciesCollection dependencies)
	{
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
		app.UseDependencies(dependencies);

		return app;
	}
}