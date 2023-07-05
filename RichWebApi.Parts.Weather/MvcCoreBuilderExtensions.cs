using Microsoft.Extensions.DependencyInjection;
using RichWebApi.Controllers;
using RichWebApi.Parts.Core;

namespace RichWebApi.Parts.Weather;

public static class MvcCoreBuilderExtensions
{
	public static IMvcCoreBuilder AddWeatherPart(this IMvcCoreBuilder builder)
		=> builder.AddPart<WeatherForecastController>();
}