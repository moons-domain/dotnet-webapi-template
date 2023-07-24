using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using RichWebApi.Services;

namespace RichWebApi.Startup;

[UsedImplicitly]
internal sealed class FillWeatherWeekAction : IAsyncStartupAction
{
	private readonly WeatherWeekFillerService _weatherWeekFillerService;
	private readonly IServiceProvider _serviceProvider;

	public uint Order => 2;

	public FillWeatherWeekAction(WeatherWeekFillerService weatherWeekFillerService, IServiceProvider serviceProvider)
	{
		_weatherWeekFillerService = weatherWeekFillerService;
		_serviceProvider = serviceProvider;
	}

	public async Task PerformActionAsync(CancellationToken cancellationToken = default)
	{
		await using var serviceFunctionScope = _serviceProvider.CreateAsyncScope();
		await _weatherWeekFillerService.PerformServiceFunctionAsync(serviceFunctionScope.ServiceProvider, cancellationToken);
	}
}