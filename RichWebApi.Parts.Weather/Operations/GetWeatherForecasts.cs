using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RichWebApi.Entities;
using RichWebApi.Extensions;
using RichWebApi.Models;
using RichWebApi.Persistence;
using RichWebApi.Utilities.Paging;

namespace RichWebApi.Operations;

public record GetWeatherForecasts(int Page, int Size, DateTime? From = null, DateTime? To = null) : IRequest<PagedResult<WeatherForecastDto>>, IPagedRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<GetWeatherForecasts>
	{
		public Validator(IValidator<IPagedRequest> validator)
		{
			Include(validator);
			RuleFor(x => x.From)
				.LessThan(x => x.To)
				.When(x => x.From is not null && x.To is not null);

			RuleFor(x => x.To)
				.GreaterThan(x => x.From)
				.When(x => x.From is not null && x.To is not null);
		}
	}

	[UsedImplicitly]
	internal class GetWeatherForecastsHandler : IRequestHandler<GetWeatherForecasts, PagedResult<WeatherForecastDto>>
	{
		private readonly IRichWebApiDatabase _database;
		private readonly IMapper _mapper;

		public GetWeatherForecastsHandler(IRichWebApiDatabase database, IMapper mapper)
		{
			_database = database;
			_mapper = mapper;
		}

		public Task<PagedResult<WeatherForecastDto>> Handle(GetWeatherForecasts request,
															CancellationToken cancellationToken)
			=> _database.ReadAsync((db, ct) => db.Context
				.Set<WeatherForecast>()
				.MaybeWhere(request.From is not null, x => x.Date >= request.From)
				.MaybeWhere(request.To is not null, x => x.Date <= request.To)
				.OrderBy(x => x.Date)
				.AsNoTracking()
				.ProjectTo<WeatherForecastDto>(_mapper.ConfigurationProvider)
				.ToPagedResultAsync(request, ct), cancellationToken);
	}
}