using FluentValidation;
using JetBrains.Annotations;
using RichWebApi.Validation;

namespace RichWebApi.Config;

public class WeatherWeekFillerServiceConfig
{
	public string Schedule { get; set; } = null!;

	[UsedImplicitly]
	public class Validator : OptionsValidator<WeatherWeekFillerServiceConfig>
	{
		public Validator() => RuleFor(x => x.Schedule).NotNull().NotEmpty().CronExpression();
	}
}