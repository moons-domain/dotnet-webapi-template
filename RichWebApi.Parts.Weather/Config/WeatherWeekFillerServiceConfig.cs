using FluentValidation;
using JetBrains.Annotations;
using RichWebApi.Validation;

namespace RichWebApi.Config;

public class WeatherWeekFillerServiceConfig : IAppConfig
{
	public string Schedule { get; set; } = null!;

	[UsedImplicitly]
	internal class Validator : AbstractValidator<WeatherWeekFillerServiceConfig>
	{
		public Validator() => RuleFor(x => x.Schedule).NotNull().NotEmpty().CronExpression();
	}
}