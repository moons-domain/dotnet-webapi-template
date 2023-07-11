﻿using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RichWebApi.Validation;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddOptionsWithValidator<TOptions, TValidator>(
		this IServiceCollection services,
		string configurationSection)
		where TOptions : class
		where TValidator : AbstractValidator<TOptions>
	{
		services.TryAddTransient<IValidator<TOptions>, TValidator>();
		services
			.AddOptions<TOptions>()
			.BindConfiguration(configurationSection)
			.ValidateDataAnnotations()
			.ValidateOnStart();
		services.TryAddSingleton<IValidateOptions<TOptions>>(x
			=> new FluentOptionValidator<TOptions>(configurationSection,
				x.GetRequiredService<IValidator<TOptions>>(),
				x.GetRequiredService<IHostEnvironment>(),
				x.GetRequiredService<ILogger<FluentOptionValidator<TOptions>>>()));
		return services;
	}
}