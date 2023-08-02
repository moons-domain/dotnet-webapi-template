using Microsoft.Extensions.DependencyInjection;

namespace RichWebApi;

public interface IAppPart
{
	void ConfigureServices(IServiceCollection services);
}