using RichWebApi.Hubs;

namespace RichWebApi;

public static class SignalRConfiguratorExtensions
{
	public static ISignalRConfigurator AddWeather(this ISignalRConfigurator configurator)
		=> configurator.WithHub<WeatherHub>("/ws/weather");
}