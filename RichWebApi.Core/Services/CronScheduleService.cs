using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace RichWebApi.Services;

public abstract class CronScheduleService : IHostedService
{

	private readonly CancellationTokenSource _cts = new();

	// ReSharper disable once NotAccessedField.Local
	private Task _task = Task.CompletedTask;
	private readonly AsyncServiceScope _serviceProviderScope;

	protected CronScheduleService(IServiceProvider serviceProvider) => _serviceProviderScope = serviceProvider.CreateAsyncScope();

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_task = Task.Run(async () =>
		{
			var logger = _serviceProviderScope.ServiceProvider.GetRequiredService<ILogger<CronScheduleService>>();
			var systemClock = _serviceProviderScope.ServiceProvider.GetRequiredService<ISystemClock>();
			var serviceName = GetType().Name;

			using (logger.BeginScope(serviceName))
			{
				while (!_cts.IsCancellationRequested)
				{
					var currentSchedule = GetSchedule();
					var now = systemClock.UtcNow.UtcDateTime;
					var nextOccurrence = currentSchedule.GetNextOccurrence(now);
					var timeToWait = nextOccurrence - now;
					logger.LogInformation(
						"Scheduling with schedule {Schedule} - next pass scheduled for {NextPass}, waiting {TimeToWait}...",
						currentSchedule.ToString(),
						nextOccurrence.ToString("O"),
						timeToWait.ToString("g"));

					await Task.Delay(timeToWait, _cts.Token);

					try
					{
						await using var functionScope = _serviceProviderScope.ServiceProvider.CreateAsyncScope();
						await PerformServiceFunctionAsync(functionScope.ServiceProvider, _cts.Token);
					}
					catch (Exception e)
					{
						logger.LogError(e, "{Service} failed to perform it's function", serviceName);
					}
				}
			}
		}, _cts.Token);

		return Task.CompletedTask;
	}

	protected abstract Task PerformServiceFunctionAsync(IServiceProvider serviceProvider,
														CancellationToken cancellationToken);

	public async Task ExecuteFunctionAsync(CancellationToken cancellationToken)
	{
		await using var functionScope = _serviceProviderScope.ServiceProvider.CreateAsyncScope();
		await PerformServiceFunctionAsync(functionScope.ServiceProvider, cancellationToken);
	}

	protected abstract CrontabSchedule GetSchedule();

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_cts.Cancel();
		return _serviceProviderScope.DisposeAsync().AsTask();
	}
}