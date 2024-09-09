using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RichWebApi.Extensions;

namespace RichWebApi.Startup;

internal sealed class StartupActionCoordinator(
	IServiceProvider serviceProvider,
	ILogger<StartupActionCoordinator> logger)
	: IStartupActionCoordinator
{
	public Task PerformStartupActionsAsync(CancellationToken cancellationToken) => logger.TimeAsync(async () =>
	{
		var actions = serviceProvider
			.GetServices<IAsyncStartupAction>()
			.OrderBy(x => x.Order)
			.ToList();

		if (actions.Count == 0)
		{
			return;
		}

		logger.LogDebug("Will run {StartupActionCount} startup actions: {@StartupActions}", actions.Count, actions.Select(a => new { a.GetType().Name, a.Order }));
		foreach (var actionGroup in actions.GroupBy(x => x.Order))
		{
			await logger.TimeAsync(() =>
			{
				var tasks = actionGroup.Select(action => logger.TimeAsync(
					() => action.PerformActionAsync(cancellationToken),
					"run startup action '{StartupActionName}'", action.GetType().Name));
				return Task.WhenAll(tasks);
			}, "run startup action group '{StartupActionGroupKey}'", actionGroup.Key);
		}
	}, "startup");
}