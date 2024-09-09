using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using RichWebApi.Entities;
using RichWebApi.Extensions;
using RichWebApi.Models;
using RichWebApi.Persistence;

namespace RichWebApi.Operations;

public record GetWeatherForecast(DateTime Date) : IRequest<WeatherForecastDto>
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<GetWeatherForecast>
	{
		public Validator() => RuleFor(x => x.Date).NotEqual(default(DateTime));
	}

	[UsedImplicitly]
	internal class GetWeatherForecastHandler : IRequestHandler<GetWeatherForecast, WeatherForecastDto>
	{
		private readonly IRichWebApiDatabase _database;
		private readonly IMapper _mapper;

		public GetWeatherForecastHandler(IRichWebApiDatabase database, IMapper mapper)
		{
			_database = database;
			_mapper = mapper;
		}

		public Task<WeatherForecastDto> Handle(GetWeatherForecast request, CancellationToken cancellationToken)
			=> _database.ReadAsync((db, ct) => db.Context
				.Set<WeatherForecast>()
				.Where(x => x.Date == request.Date)
				.ProjectTo<WeatherForecastDto>(_mapper.ConfigurationProvider)
				.FirstOrExceptionAsync(ct), cancellationToken);
	}
}