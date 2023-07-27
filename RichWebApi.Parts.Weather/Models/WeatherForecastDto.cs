using FluentValidation;
using JetBrains.Annotations;

namespace RichWebApi.Models;

public class WeatherForecastDto
{
	public DateTime Date { get; set; }

	public int TemperatureC { get; set; }

	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

	public string Summary { get; set; } = null!;

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