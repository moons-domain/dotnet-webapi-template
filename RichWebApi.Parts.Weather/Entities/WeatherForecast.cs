using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RichWebApi.Entities.Configuration;

namespace RichWebApi.Entities;

[Table("WeatherForecasts", Schema = "weather")]
[Comment("Weather per day forecasts")]
public class WeatherForecast : IAuditableEntity, ISoftDeletableEntity
{
	public long WeatherForecastId { get; set; }

	public DateTime Date { get; set; }

	[Range(-100, 100)] public int TemperatureC { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime ModifiedAt { get; set; }

	public DateTime? DeletedAt { get; set; }

	[MaxLength(500)] public string Summary { get; set; } = null!;

	[UsedImplicitly]
	public class Validator : AbstractValidator<WeatherForecast>
	{
		public Validator(IValidator<IAuditableEntity> aeValidator, IValidator<ISoftDeletableEntity> sdeValidator)
		{
			Include(aeValidator);
			Include(sdeValidator);
			RuleFor(x => x.Date)
				.Must(d => d is { Hour: 0, Minute: 0, Second: 0, Millisecond: 0, Microsecond: 0 })
				.WithMessage("Weather date should contain only year, month and day");
			RuleFor(x => x.TemperatureC).InclusiveBetween(-100, 100);
		}
	}

	public class Configurator : EntityConfiguration<WeatherForecast>
	{
		public override void Configure(EntityTypeBuilder<WeatherForecast> builder)
		{
			base.Configure(builder);
			builder.HasKey(x => x.WeatherForecastId);
			builder.Property(x => x.Date).HasColumnType("date");
			builder.HasIndex(x => new { x.Date, x.DeletedAt })
				.IsUnique()
				.HasFilter(null);
			builder.HasData(new WeatherForecast
			{
				WeatherForecastId = 1,
				Date = new DateTime(2023, 7, 15),
				Summary = "Freezing",
				TemperatureC = 18,
				CreatedAt = new DateTime(2023, 7, 14, 1, 11, 11),
				ModifiedAt = new DateTime(2023, 7, 14, 1, 11, 11)
			});
		}
	}
}