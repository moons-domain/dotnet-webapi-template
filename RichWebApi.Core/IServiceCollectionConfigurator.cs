using Microsoft.Extensions.DependencyInjection;

namespace RichWebApi;

public interface IServiceCollectionConfigurator
{
	void ConfigureServices(IServiceCollection services);
}