using JetBrains.Annotations;
using RichWebApi.Services;

namespace RichWebApi.Startup;

[UsedImplicitly]
internal class FillWeatherWeekAction : IAsyncStartupAction
{
	private readonly WeatherWeekFillerService _weatherWeekFillerService;

	public uint Order => 2;

	public FillWeatherWeekAction(WeatherWeekFillerService weatherWeekFillerService)
		=> _weatherWeekFillerService = weatherWeekFillerService;

	public Task PerformActionAsync(CancellationToken cancellationToken = default)
		=> _weatherWeekFillerService.ExecuteFunctionAsync(cancellationToken);
}