using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Internal;
using RichWebApi.Config;
using RichWebApi.Maintenance;
using RichWebApi.MediatR;
using RichWebApi.Services;
using RichWebApi.Startup;

namespace RichWebApi;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCore(this IServiceCollection services)
	{
		services.TryAddTransient<IStartupActionCoordinator, StartupActionCoordinator>();
		services.TryAddSingleton<IApplicationMaintenance, ApplicationMaintenance>();
		services.TryAddSingleton<ISystemClock, SystemClock>();
		return services;
	}

	public static IServiceCollection AddStartupAction<T>(this IServiceCollection services) where T : IAsyncStartupAction
	{
		services.TryAddEnumerable(new ServiceDescriptor(typeof(IAsyncStartupAction), typeof(T),
			ServiceLifetime.Scoped));
		return services;
	}

	public static IServiceCollection AddCronService<T>(this IServiceCollection services)
		where T : CronScheduleService
	{
		services.TryAddSingleton<T>();
		services.AddHostedService<T>();
		return services;
	}

	public static IServiceCollection CollectCoreServicesFromAssemblies(this IServiceCollection services,
	                                                                   Assembly[] assemblies)
	{
		var optionsValidatorTagType = typeof(IOptionsValidator);
		return services.AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true,
				filter: result => !result.ValidatorType.IsAssignableTo(optionsValidatorTagType)) // options validators have their own lifetime
			.AddMediatR(x => x.RegisterServicesFromAssemblies(assemblies))
			.AddAutoMapper(assemblies);
	}

	public static IServiceCollection AddCoreMediatRBehaviors(this IServiceCollection services)
	{
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceLoggingBehavior<,>));
		return services;
	}
}