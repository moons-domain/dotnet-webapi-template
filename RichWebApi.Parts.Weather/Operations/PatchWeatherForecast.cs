﻿using AutoMapper;
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
	internal class PatchWeatherForecastHandler(
		IRichWebApiDatabase database,
		IMapper mapper,
		IHubContext<WeatherHub, IWeatherHubClient> hubContext)
		: IRequestHandler<PatchWeatherForecast>
	{
		public async Task Handle(PatchWeatherForecast request, CancellationToken cancellationToken)
		{
			var forecast = request.WeatherForecast;
			var foundForecast = await database.ReadAsync((db, ct) => db.Context
					.Set<WeatherForecast>()
					.FirstOrExceptionAsync(x => x.Date == forecast.Date, ct), cancellationToken
			);

			mapper.Map(forecast, foundForecast);
			await database.PersistAsync(cancellationToken);
			await hubContext.Clients
				.Group(WeatherHubConstants.GroupName)
				.OnWeatherUpdate(forecast);
		}
	}
}