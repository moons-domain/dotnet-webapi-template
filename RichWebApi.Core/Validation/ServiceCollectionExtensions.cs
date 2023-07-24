using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RichWebApi.Config;

namespace RichWebApi.Validation;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddOptionsWithValidator<TOptions, TValidator>(
		this IServiceCollection services,
		string configurationSection)
		where TOptions : class, new()
		where TValidator : OptionsValidator<TOptions>
	{
		services.TryAddTransient<IValidator<TOptions>, TValidator>();
		services.TryAddSingleton<IValidateOptions<TOptions>>(x
			=> new FluentOptionValidator<TOptions>(configurationSection,
				x.GetRequiredService<IValidator<TOptions>>(),
				x.GetRequiredService<IWebHostEnvironment>(),
				x.GetRequiredService<ILogger<FluentOptionValidator<TOptions>>>()));
		services
			.AddOptions<TOptions>()
			.BindConfiguration(configurationSection)
			.ValidateDataAnnotations()
			.ValidateOnStart();
		return services;
	}
}