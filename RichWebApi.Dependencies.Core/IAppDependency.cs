using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RichWebApi;

public interface IAppDependency
{
	public void ConfigureServices(IServiceCollection services);

	public void ConfigureApplication(IApplicationBuilder builder);
}