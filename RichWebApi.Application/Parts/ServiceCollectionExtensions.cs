using System.Reflection;
using FluentValidation;

namespace RichWebApi.Parts;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection EnrichWithApplicationParts(this IServiceCollection services,
																IAppPartsCollection parts)
	{
		var partsArray = parts.ToArray();
		var mvcCoreBuilder = services.AddMvcCore();
		var assembliesList = new Assembly[parts.Count];
		for (var i = 0; i < partsArray.Length; i++)
		{
			var part = partsArray[i];
			part.ConfigureServices(services);

			var assembly = part.GetType().Assembly;
			mvcCoreBuilder.AddApplicationPart(assembly);

			assembliesList[i] = assembly;
		}

		services.AddValidatorsFromAssemblies(assembliesList, includeInternalTypes: true)
			.AddMediatR(x => x.RegisterServicesFromAssemblies(assembliesList))
			.AddSwaggerGen(s => s.AddSignalRSwaggerGen(options => options.ScanAssemblies(assembliesList)))
			.AddAutoMapper(assembliesList);
		return services;
	}
}