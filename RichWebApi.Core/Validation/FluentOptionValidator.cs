using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RichWebApi.Validation;

public class FluentOptionValidator<T> : IValidateOptions<T>
	where T : class
{
	private readonly string _configurationSection;
	private readonly IValidator<T> _validator;
	private readonly IWebHostEnvironment _environment;
	private readonly ILogger<FluentOptionValidator<T>> _logger;

	public FluentOptionValidator(string configurationSection,
		IValidator<T> validator,
		IWebHostEnvironment environment,
		ILogger<FluentOptionValidator<T>> logger)
	{
		_configurationSection = configurationSection;
		_validator = validator;
		_environment = environment;
		_logger = logger;
	}

	public ValidateOptionsResult Validate(string? name, T options)
	{
		var result = _validator.Validate(options);
		if (result.IsValid)
		{
			if (_environment.IsDevelopment())
			{
				_logger.LogDebug("Config section {ConfigSection} is valid, value {@Value}", _configurationSection,
					options);
			}
			else
			{
				_logger.LogDebug("Config section {ConfigSection} is valid", _configurationSection);
			}

			return ValidateOptionsResult.Success;
		}
		var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
		var errorMessage = $"Config section '{_configurationSection}' is invalid, reasons:"
						   + Environment.NewLine
						   + string.Join(Environment.NewLine, errorMessages);

		_logger.LogError("Config section '{ConfigurationSection}' is invalid, reasons: {@Reasons}",
			_configurationSection,
			errorMessages);
		return ValidateOptionsResult.Fail(errorMessage);
	}
}