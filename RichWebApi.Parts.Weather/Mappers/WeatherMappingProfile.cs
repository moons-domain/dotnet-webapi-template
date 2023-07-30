using AutoMapper;
using JetBrains.Annotations;
using RichWebApi.Entities;
using RichWebApi.Models;

namespace RichWebApi.Mappers;

[UsedImplicitly]
public class WeatherMappingProfile : Profile
{
	public WeatherMappingProfile()
		=> CreateMap<WeatherForecast, WeatherForecastDto>(MemberList.Destination)
			.ForMember(x => x.TemperatureF, x => x.Ignore())
			.ReverseMap()
			.IgnoreAuditableProperties();
}