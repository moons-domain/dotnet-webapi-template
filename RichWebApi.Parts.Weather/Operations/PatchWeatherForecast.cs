using AutoMapper;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RichWebApi.Entities;
using RichWebApi.Extensions;
using RichWebApi.Models;
using RichWebApi.Persistence;

namespace RichWebApi.Operations;

public record PatchWeatherForecast(WeatherForecastDto WeatherForecast) : IRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<PatchWeatherForecast>
	{
		public Validator(IValidator<WeatherForecastDto> validator)
		{
			RuleFor(x => x.WeatherForecast).SetValidator(validator);
		}
	}

	[UsedImplicitly]
	public class PutWeatherForecastHandler : IRequestHandler<PatchWeatherForecast>
	{
		private readonly IRichWebApiDatabase _database;
		private readonly IMapper _mapper;

		public PutWeatherForecastHandler(IRichWebApiDatabase database, IMapper mapper)
		{
			_database = database;
			_mapper = mapper;
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
		}
	}
}