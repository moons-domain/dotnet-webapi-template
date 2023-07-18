using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RichWebApi.Entities.Configuration;
using RichWebApi.Persistence;

namespace RichWebApi;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSaveChangesReactor<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
		where T : class, ISaveChangesReactor
	{
		services.TryAddEnumerable(new ServiceDescriptor(typeof(ISaveChangesReactor), typeof(T),
			lifetime));
		return services;
	}

	public static IServiceCollection CollectDatabaseEntities(this IServiceCollection services, Assembly[] assemblies)
	{
		var entityConfigurationType = typeof(INonGenericEntityConfiguration);
		foreach (var assembly in assemblies)
		{
			var configurations = assembly.GetExportedTypes()
				.Where(x => x.IsAssignableTo(entityConfigurationType))
				.Select(c => new ServiceDescriptor(entityConfigurationType, c, ServiceLifetime.Scoped));
			services.TryAddEnumerable(configurations);
		}

		return services;
	}
}