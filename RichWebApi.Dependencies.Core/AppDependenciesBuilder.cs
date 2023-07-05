using Microsoft.Extensions.DependencyInjection;

namespace RichWebApi;

internal class AppDependenciesBuilder : IAppDependenciesBuilder
{
	private readonly IServiceCollection _services;

	public AppDependenciesBuilder(IServiceCollection services)
	{
		_services = services;
	}

	public IAppDependenciesBuilder AddServices(Action<IServiceCollection> configure)
	{
		configure.Invoke(_services);
		return this;
	}
}