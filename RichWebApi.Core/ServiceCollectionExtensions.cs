using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Internal;
using RichWebApi.Maintenance;
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
}