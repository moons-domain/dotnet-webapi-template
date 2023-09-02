using Microsoft.Extensions.DependencyInjection;
using NCrontab;
using NSubstitute;
using RichWebApi.Services;
using RichWebApi.Tests;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Logging;
using RichWebApi.Tests.Moq;
using Xunit.Abstractions;

namespace RichWebApi.Core.Tests.Unit.Services;

public class CronScheduleServiceTests : UnitTest
{
	private readonly DependencyContainerFixture _container;

	public CronScheduleServiceTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container) :
		base(testOutputHelper)
	{
		_container = container
			.WithXunitLogging(TestOutputHelper);
	}

	[Fact(Skip = "takes some time")]
	public async Task CallsServiceFunction()
	{
		var sp = _container
			.ReplaceWithMock<IUnitService>(mock => mock.DoSomethingAsync()
				.Returns(Task.CompletedTask))
			.BuildServiceProvider();
		var schedule = CrontabSchedule.Parse("* * * * *");
		var scheduledService = new UnitCronScheduleService(sp, schedule);

		await scheduledService.StartAsync(default);
		await Task.Delay(schedule.GetNextOccurrence(DateTime.UtcNow)
						 - DateTime.UtcNow
						 + TimeSpan.FromSeconds(2)); // wait until performing of service function
		await scheduledService.StopAsync(default);

		var mock = sp.GetRequiredService<IUnitService>();
		await mock.Received(1).DoSomethingAsync();
	}

	// ReSharper disable once MemberCanBePrivate.Global - it's being used by Moq
	public interface IUnitService
	{
		Task DoSomethingAsync();
	}

	private class UnitCronScheduleService : CronScheduleService
	{
		private readonly CrontabSchedule _schedule;

		public UnitCronScheduleService(IServiceProvider serviceProvider, CrontabSchedule schedule) : base(
			serviceProvider)
		{
			_schedule = schedule;
		}

		public override Task PerformServiceFunctionAsync(IServiceProvider serviceProvider,
														 CancellationToken cancellationToken)
			=> serviceProvider.GetRequiredService<IUnitService>().DoSomethingAsync();

		protected override CrontabSchedule GetSchedule()
			=> _schedule;
	}
}