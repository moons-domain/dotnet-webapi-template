using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RichWebApi;

public interface IAppDependency
{
	void ConfigureServices(IServiceCollection services, IAppPartsCollection parts);
	void ConfigureApplication(IApplicationBuilder builder);
}