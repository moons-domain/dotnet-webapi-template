using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RichWebApi.Extensions;

namespace RichWebApi.Startup;

internal sealed class StartupActionCoordinator : IStartupActionCoordinator
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<StartupActionCoordinator> _logger;

	public StartupActionCoordinator(IServiceProvider serviceProvider, ILogger<StartupActionCoordinator> logger)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	public Task PerformStartupActionsAsync(CancellationToken cancellationToken) => _logger.TimeAsync(async () =>
	{
		var actions = _serviceProvider
			.GetServices<IAsyncStartupAction>()
			.OrderBy(x => x.Order)
			.ToList();

		if (actions.Count == 0)
		{
			return;
		}

		_logger.LogDebug("Will run {StartupActionCount} startup actions: {@StartupActions}", actions.Count, actions.Select(a => new { a.GetType().Name, a.Order }));
		foreach (var actionGroup in actions.GroupBy(x => x.Order))
		{
			await _logger.TimeAsync(() =>
			{
				var tasks = actionGroup.Select(action => _logger.TimeAsync(
					() => action.PerformActionAsync(cancellationToken),
					"run startup action '{StartupActionName}'", action.GetType().Name));
				return Task.WhenAll(tasks);
			}, "run startup action group '{StartupActionGroupKey}'", actionGroup.Key);
		}
	}, "startup");
}