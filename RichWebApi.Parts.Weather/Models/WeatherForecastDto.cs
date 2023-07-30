using FluentValidation;
using JetBrains.Annotations;
using RichWebApi.Entities;

namespace RichWebApi.Models;

public class WeatherForecastDto : IAuditableDto
{
	public DateTime Date { get; set; }

	public int TemperatureC { get; set; }

	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

	public string Summary { get; set; } = null!;

	public DateTime CreatedAt { get; set; }

	public DateTime ModifiedAt { get; set; }

	[UsedImplicitly]
	public class Validator : AbstractValidator<WeatherForecastDto>
	{
		public Validator()
		{
			RuleFor(x => x.Date).GreaterThanOrEqualTo(new DateTime(2023, 1, 1, 0, 0, 0, 0, 0));
			RuleFor(x => x.TemperatureC).InclusiveBetween(-100, 100);
			RuleFor(x => x.Summary).NotNull().NotEmpty().MaximumLength(500);
		}
	}
}