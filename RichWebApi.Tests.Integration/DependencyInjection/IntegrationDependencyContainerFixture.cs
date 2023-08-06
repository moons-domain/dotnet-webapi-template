using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RichWebApi.Tests.Client;
using RichWebApi.Tests.Config;

namespace RichWebApi.Tests.DependencyInjection;

[UsedImplicitly]
public class IntegrationDependencyContainerFixture : DependencyContainerFixture
{
	private static readonly IReadOnlyList<ClientDescriptor> apiClientsToRegister;

	static IntegrationDependencyContainerFixture()
		=> apiClientsToRegister = ScanForClients(typeof(IntegrationTest).Assembly);

	protected override IServiceCollection ConfigureSharedServices(IServiceCollection services)
	{
		base.ConfigureSharedServices(services);
		var configuration = new ConfigurationBuilder()
			.AddUserSecrets<IntegrationTest>()
			.Build();
		services.TryAddSingleton<IConfiguration>(configuration);
		services.AddOptions<IntegrationTestsConfig>()
			.BindConfiguration("Config");
		services.AddHttpClient<IWeatherForecastClient, WeatherForecastClient>(static (sp, client) =>
		{
			var options = sp.GetRequiredService<IOptions<IntegrationTestsConfig>>();
			client.BaseAddress = new Uri(options.Value.BaseUrl);
		});
		foreach (var descriptor in apiClientsToRegister)
		{
			descriptor.AddClientToServices(services, static (sp, client) =>
			{
				var options = sp.GetRequiredService<IOptions<IntegrationTestsConfig>>();
				client.BaseAddress = new Uri(options.Value.BaseUrl);
			});
		}

		return services;
	}


	private static IReadOnlyList<ClientDescriptor> ScanForClients(Assembly assembly)
	{
		var baseClassType = typeof(RichWebApiClient);
		var interfaceType = typeof(IRichWebApiClient);
		var baseClassInheritors =
			assembly.GetTypes()
				.Where(x => x is { IsClass: true, IsAbstract: false } && x.IsAssignableTo(baseClassType))
				.ToArray();
		var addHttpClientMethod = typeof(HttpClientFactoryServiceCollectionExtensions)
			.GetMethods(BindingFlags.Public | BindingFlags.Static)
			.Single(x =>
			{
				if (x is not
					{
						Name: nameof(HttpClientFactoryServiceCollectionExtensions.AddHttpClient),
						IsGenericMethodDefinition: true
					})
				{
					return false;
				}

				if (x.GetGenericArguments().Length != 2)
				{
					return false;
				}

				var parameters = x.GetParameters();
				if (parameters.Length != 2)
				{
					return false;
				}

				return parameters[1].ParameterType == typeof(Action<IServiceProvider, HttpClient>);
			});
		return baseClassInheritors
			.Select(x =>
			{
				var serviceType = x.GetInterfaces().Single(i => i != interfaceType && i.IsAssignableTo(interfaceType));
				var addSpecificHttpClient = addHttpClientMethod.MakeGenericMethod(serviceType, x);
				return new ClientDescriptor(serviceType,
					x,
					(services, configure) => addSpecificHttpClient.Invoke(null, new object?[] { services, configure }));
			})
			.ToList()
			.AsReadOnly();
	}

	private record ClientDescriptor(Type ServiceType, Type ImplementationType, Action<IServiceCollection, Action<IServiceProvider, HttpClient>> AddClientToServices);
}