using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
}