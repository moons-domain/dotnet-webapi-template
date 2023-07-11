using FluentValidation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RichWebApi.Validation;

public class FluentOptionValidator<T> : IValidateOptions<T>
	where T : class
{
	private readonly string _configurationSection;
	private readonly IValidator<T> _validator;
	private readonly IHostEnvironment _environment;
	private readonly ILogger<FluentOptionValidator<T>> _logger;

	public FluentOptionValidator(string configurationSection,
		IValidator<T> validator,
		IHostEnvironment environment,
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

		var firstError = result.Errors.First();
		var value = result
			.Errors
			.Select(x => x.AttemptedValue)
			.FirstOrDefault(x => x != null);
		var configurationKey = _configurationSection + ":" + firstError.PropertyName;
		var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
		var errorMessage = $"Config key {configurationKey} contains an invalid value of \"{value}\", reasons:"
						   + Environment.NewLine
						   + string.Join(Environment.NewLine, errorMessages);

		_logger.LogError("Config key {ConfigKey} contains an invalid value of \"{InvalidValue}\", reasons: {@Reasons}",
			configurationKey,
			value,
			errorMessages);
		return ValidateOptionsResult.Fail(errorMessage);
	}
}