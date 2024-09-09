using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RichWebApi;

internal class DependenciesCore : IAppDependency
{
	public void ConfigureServices(IServiceCollection services, IAppPartsCollection parts)
	{
	}

	public void ConfigureApplication(IApplicationBuilder builder)
	{
	}
}