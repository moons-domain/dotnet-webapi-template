using Microsoft.Extensions.DependencyInjection;

namespace RichWebApi;

public interface IAppDependenciesBuilder
{
	public IAppDependenciesBuilder AddServices(Action<IServiceCollection> configure);

}