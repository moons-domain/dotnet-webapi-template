using Microsoft.AspNetCore.Builder;

namespace RichWebApi;

public interface IAppDependency : IServiceCollectionConfigurator
{
	void ConfigureApplication(IApplicationBuilder builder);
}