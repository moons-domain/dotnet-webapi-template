using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RichWebApi.Config;

namespace RichWebApi.Validation;

internal class FluentOptionValidator<T>(
	string configurationSection,
	IValidator<T> validator,
	IWebHostEnvironment environment,
	ILogger<FluentOptionValidator<T>> logger)
	: IValidateOptions<T>
	where T : class, IAppConfig
{
	public ValidateOptionsResult Validate(string? name, T options)
	{
		var result = validator.Validate(options);
		if (result.IsValid)
		{
			if (environment.IsDevelopment())
			{
				logger.LogDebug("Config section {ConfigSection} is valid, value {@Value}", configurationSection,
					options);
			}
			else
			{
				logger.LogDebug("Config section {ConfigSection} is valid", configurationSection);
			}

			return ValidateOptionsResult.Success;
		}
		var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
		var errorMessage = $"Config section '{configurationSection}' is invalid, reasons:"
						   + Environment.NewLine
						   + string.Join(Environment.NewLine, errorMessages);

		logger.LogError("Config section '{ConfigurationSection}' is invalid, reasons: {@Reasons}",
			configurationSection,
			errorMessages);
		return ValidateOptionsResult.Fail(errorMessage);
	}
}