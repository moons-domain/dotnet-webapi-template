using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using RichWebApi.Models;

namespace RichWebApi.Operations;

public record GetWeatherForecast : IRequest<ICollection<WeatherForecast>>
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<GetWeatherForecast>
	{
	}

	[UsedImplicitly]
	public class GetWeatherForecastHandler : IRequestHandler<GetWeatherForecast, ICollection<WeatherForecast>>
	{
		private static readonly string[] summaries = {
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		public Task<ICollection<WeatherForecast>> Handle(GetWeatherForecast request,
														 CancellationToken cancellationToken)
		{
			ICollection<WeatherForecast> result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = summaries[Random.Shared.Next(summaries.Length)]
			})
				.ToArray();
			return Task.FromResult(result);
		}
	}
}