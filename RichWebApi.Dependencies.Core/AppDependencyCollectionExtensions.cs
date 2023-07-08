using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RichWebApi;

public static class AppDependencyCollectionExtensions
{
	public static IServiceCollection AddDependencyServices(this IServiceCollection services, IReadOnlyCollection<IAppDependency> dependencies)
	{
		foreach (var d in dependencies)
		{
			d.ConfigureServices(services);
		}

		return services;
	}

	public static IApplicationBuilder UseDependencies(this IApplicationBuilder builder, IReadOnlyCollection<IAppDependency> dependencies)
	{
		foreach (var d in dependencies)
		{
			d.ConfigureApplication(builder);
		}

		return builder;
	}
}