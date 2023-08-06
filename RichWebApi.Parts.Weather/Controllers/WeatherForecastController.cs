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
	public Task<PagedResult<WeatherForecastDto>> GetWeatherForecastsPage([FromQuery(Name = "page")] int page,
																		 [FromQuery(Name = "size")] int size,
																		 [FromQuery(Name = "fromDate")] DateTime? from,
																		 [FromQuery(Name = "toDate")] DateTime? to,
																		 CancellationToken ct)
		=> _mediator.Send(new GetWeatherForecasts(page, size, from, to), ct);

	[HttpGet("{date:datetime}", Name = "GetWeatherForecast")]
	public Task<WeatherForecastDto> GetWeatherForecastByDate([FromRoute(Name = nameof(date))] DateTime date,
															 CancellationToken ct)
		=> _mediator.Send(new GetWeatherForecast(date), ct);

	[HttpPatch(Name = "PatchWeatherForecast")]
	public Task PatchWeatherForecast([FromBody] WeatherForecastDto weatherForecast, CancellationToken ct)
		=> _mediator.Send(new PatchWeatherForecast(weatherForecast), ct);
}