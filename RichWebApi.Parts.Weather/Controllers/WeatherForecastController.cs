using MediatR;
using Microsoft.AspNetCore.Mvc;
using RichWebApi.Models;
using RichWebApi.Operations;

namespace RichWebApi.Controllers;

[ApiController]
[Route("weatherForecasts")]
public class WeatherForecastController : ControllerBase
{
	private readonly IMediator _mediator;

	public WeatherForecastController(IMediator mediator) => _mediator = mediator;

	[HttpGet(Name = "GetWeatherForecast")]
	public Task<ICollection<WeatherForecastDto>> Get(CancellationToken ct)
		=> _mediator.Send(new GetWeatherForecast(), ct);
}