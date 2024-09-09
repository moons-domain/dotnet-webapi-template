using FluentValidation;
using FluentValidation.Results;
using NCrontab;

namespace RichWebApi.Validation;

public static class RuleBuilderExtensions
{
	public static IRuleBuilder<T, string> CronExpression<T>(this IRuleBuilder<T, string> builder) => builder
		.Custom((value, context) =>
		{
			try
			{
				var _ = CrontabSchedule.Parse(value);
			}
			catch
			{
				context.AddFailure(
					new ValidationFailure(
						context.PropertyPath,
						$"'{context.PropertyPath}' does not contain a valid CRON expression.",
						value));
			}
		});
}