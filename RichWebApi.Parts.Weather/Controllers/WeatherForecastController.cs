using MediatR;
using Microsoft.AspNetCore.Mvc;
using RichWebApi.Models;
using RichWebApi.Operations;
using RichWebApi.Utilities.Paging;

namespace RichWebApi.Controllers;

[ApiController]
[Route("weatherForecasts")]
public class WeatherForecastController : ControllerBase
{
	private readonly IMediator _mediator;

	public WeatherForecastController(IMediator mediator) => _mediator = mediator;

	[HttpGet(Name = "GetWeatherForecasts")]
	public Task<PagedResult<WeatherForecastDto>> Get([FromQuery(Name = "page")] int page,
													 [FromQuery(Name = "size")] int size, CancellationToken ct)
		=> _mediator.Send(new GetWeatherForecast(page, size), ct);

	[HttpPatch(Name = "PatchWeatherForecast")]
	public Task PatchWeatherForecast([FromBody] WeatherForecastDto weatherForecast, CancellationToken ct)
		=> _mediator.Send(new PatchWeatherForecast(weatherForecast), ct);
}