using Microsoft.Extensions.DependencyInjection;
using RichWebApi.Config;
using RichWebApi.Services;
using RichWebApi.Startup;
using RichWebApi.Validation;

namespace RichWebApi;

internal class WeatherPart : IAppPart
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddOptionsWithValidator<WeatherWeekFillerServiceConfig, WeatherWeekFillerServiceConfig.Validator>(
			"Services:WeatherWeekFiller");
		services.AddCronService<WeatherWeekFillerService>();
		services.AddStartupAction<FillWeatherWeekAction>();
	}
}