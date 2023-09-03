using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RichWebApi.Startup;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Logging;
using RichWebApi.Tests.NSubstitute;
using Xunit.Abstractions;

namespace RichWebApi.Tests.Startup;

public class StartupActionCoordinatorTests : UnitTest
{
	private readonly DependencyContainerFixture _container;

	public StartupActionCoordinatorTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container) :
		base(testOutputHelper)
	{
		_container = container
			.WithXunitLogging(TestOutputHelper)
			.ConfigureServices(s => s.AddCore());
	}

	[Fact]
	public async Task CallsStartupAction()
	{
		var sp = _container
			.ReplaceWithMock<IAsyncStartupAction>(mock
				=> mock.PerformActionAsync(Arg.Any<CancellationToken>())
					.Returns(Task.CompletedTask))
			.BuildServiceProvider();
		var coordinator = sp.GetRequiredService<IStartupActionCoordinator>();
		await coordinator.PerformStartupActionsAsync(default);

		var startupActionMock = sp.GetRequiredService<IAsyncStartupAction>();
		await startupActionMock.Received(1).PerformActionAsync(Arg.Any<CancellationToken>());
	}
}