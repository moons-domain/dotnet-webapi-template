using RichWebApi.Models;
using SignalRSwaggerGen.Attributes;

namespace RichWebApi.Hubs;

[SignalRHub("/ws/weather", description: "SignalR hub")]
internal interface IWeatherHubClient
{
	[SignalRHidden]
	Task OnWeatherUpdate(WeatherForecastDto weatherForecast);
}