using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using RichWebApi.Entities;
using RichWebApi.Extensions;
using RichWebApi.Hubs;
using RichWebApi.Models;
using RichWebApi.Persistence;

namespace RichWebApi.Operations;

public record PatchWeatherForecast(WeatherForecastDto WeatherForecast) : IRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<PatchWeatherForecast>
	{
		public Validator(IValidator<WeatherForecastDto> validator)
			=> RuleFor(x => x.WeatherForecast).SetValidator(validator);
	}

	[UsedImplicitly]
	internal class PatchWeatherForecastHandler : IRequestHandler<PatchWeatherForecast>
	{
		private readonly IRichWebApiDatabase _database;
		private readonly IMapper _mapper;
		private readonly IHubContext<WeatherHub, IWeatherHubClient> _hubContext;

		public PatchWeatherForecastHandler(IRichWebApiDatabase database, IMapper mapper,
										   IHubContext<WeatherHub, IWeatherHubClient> hubContext)
		{
			_database = database;
			_mapper = mapper;
			_hubContext = hubContext;
		}

		public async Task Handle(PatchWeatherForecast request, CancellationToken cancellationToken)
		{
			var forecast = request.WeatherForecast;
			var foundForecast = await _database.ReadAsync((db, ct) => db.Context
					.Set<WeatherForecast>()
					.FirstOrExceptionAsync(x => x.Date == forecast.Date, ct), cancellationToken
			);

			_mapper.Map(forecast, foundForecast);
			await _database.PersistAsync(cancellationToken);
			await _hubContext.Clients
				.Group(WeatherHubConstants.GroupName)
				.OnWeatherUpdate(forecast);
		}
	}
}