using Microsoft.Extensions.DependencyInjection;

namespace RichWebApi;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDependencyServices(this IServiceCollection services, IAppDependenciesCollection dependencies)
	{
		foreach (var d in dependencies)
		{
			d.ConfigureServices(services);
		}

		return services;
	}
}