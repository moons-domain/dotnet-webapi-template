using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RichWebApi.Entities;
using RichWebApi.Models;
using RichWebApi.Persistence;
using RichWebApi.Utilities.Paging;

namespace RichWebApi.Operations;

public record GetWeatherForecast(int Page, int Size) : IRequest<PagedResult<WeatherForecastDto>>, IPagedRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<GetWeatherForecast>
	{
		public Validator(IValidator<IPagedRequest> validator)
			=> Include(validator);
	}

	[UsedImplicitly]
	internal class GetWeatherForecastHandler : IRequestHandler<GetWeatherForecast, PagedResult<WeatherForecastDto>>
	{
		private readonly IRichWebApiDatabase _database;
		private readonly IMapper _mapper;

		public GetWeatherForecastHandler(IRichWebApiDatabase database, IMapper mapper)
		{
			_database = database;
			_mapper = mapper;
		}

		public Task<PagedResult<WeatherForecastDto>> Handle(GetWeatherForecast request,
															CancellationToken cancellationToken)
			=> _database.ReadAsync((db, ct) => db.Context
				.Set<WeatherForecast>()
				.OrderBy(x => x.Date)
				.AsNoTracking()
				.ProjectTo<WeatherForecastDto>(_mapper.ConfigurationProvider)
				.ToPagedResultAsync(request, ct), cancellationToken);
	}
}