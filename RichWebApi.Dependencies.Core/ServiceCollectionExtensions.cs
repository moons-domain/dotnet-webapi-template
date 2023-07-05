using Microsoft.Extensions.DependencyInjection;

namespace RichWebApi;

public static class ServiceCollectionExtensions
{
	public static IAppDependenciesBuilder AddDependencies(this IServiceCollection services,
														  Action<IAppDependenciesBuilder> configure)
	{
		var builder = new AppDependenciesBuilder(services);
		configure.Invoke(builder);
		return builder;
	}
}