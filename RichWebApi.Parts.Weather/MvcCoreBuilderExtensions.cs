using Microsoft.Extensions.DependencyInjection;
using RichWebApi.Controllers;

namespace RichWebApi;

public static class MvcCoreBuilderExtensions
{
	public static IMvcCoreBuilder AddWeatherPart(this IMvcCoreBuilder builder)
		=> builder.AddPart<WeatherForecastController>();
}