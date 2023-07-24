using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RichWebApi.Config;

namespace RichWebApi.Parts;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection EnrichWithApplicationParts(this IServiceCollection services,
																IAppPartsCollection parts)
	{
		var partsArray = parts.Select(x => new { Part = x, PartAssembly = x.GetType().Assembly }).ToArray();
		services.TryAddEnumerable(partsArray.Select(x => new ServiceDescriptor(typeof(IAppPart), x.Part)));
		var mvcCoreBuilder = services.AddMvcCore();
		var partsAssemblies = partsArray
			.Select(x => x.PartAssembly)
			.ToArray();
		var optionsValidatorTagType = typeof(IOptionsValidator);
		services.AddValidatorsFromAssemblies(partsAssemblies, includeInternalTypes: true,
				filter: result => !result.ValidatorType.IsAssignableTo(optionsValidatorTagType)) // options validators have their own lifetime
			.AddMediatR(x => x.RegisterServicesFromAssemblies(partsAssemblies))
			.AddSwaggerGen(s => s.AddSignalRSwaggerGen(options => options.ScanAssemblies(partsAssemblies)))
			.AddAutoMapper(partsAssemblies)
			.CollectDatabaseEntities(partsAssemblies);

		foreach (var p in partsArray)
		{
			mvcCoreBuilder.AddApplicationPart(p.PartAssembly);
			p.Part.ConfigureServices(services);
		}

		return services;
	}
}