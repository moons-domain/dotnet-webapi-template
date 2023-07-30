using System.Reflection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RichWebApi.Entities.Configuration;
using RichWebApi.Persistence.Interceptors;

namespace RichWebApi;

public static class ServiceCollectionExtensions
{

	public static IServiceCollection AddSaveChangesInterceptor<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
		where T : class, ISaveChangesInterceptor, IOrderedInterceptor
	{
		services.TryAddEnumerable(new ServiceDescriptor(typeof(IOrderedInterceptor), typeof(T), lifetime));
		return services;
	}

	internal static IServiceCollection CollectDatabaseEntities(this IServiceCollection services,
															   IEnumerable<Assembly> assemblies)
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