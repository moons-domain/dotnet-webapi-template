using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using RichWebApi.Services;

namespace RichWebApi.Startup;

[UsedImplicitly]
internal sealed class FillWeatherWeekAction(
	WeatherWeekFillerService weatherWeekFillerService,
	IServiceProvider serviceProvider)
	: IAsyncStartupAction
{
	public uint Order => 2;

	public async Task PerformActionAsync(CancellationToken cancellationToken = default)
	{
		await using var serviceFunctionScope = serviceProvider.CreateAsyncScope();
		await weatherWeekFillerService.PerformServiceFunctionAsync(serviceFunctionScope.ServiceProvider, cancellationToken);
	}
}