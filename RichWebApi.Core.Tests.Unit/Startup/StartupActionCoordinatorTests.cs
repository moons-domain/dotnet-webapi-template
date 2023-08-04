using Microsoft.Extensions.DependencyInjection;
using Moq;
using RichWebApi.Startup;
using RichWebApi.Tests;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Logging;
using RichWebApi.Tests.Moq;
using Xunit.Abstractions;

namespace RichWebApi.Core.Tests.Unit.Startup;

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
				=> mock.Setup(x => x.PerformActionAsync(It.IsAny<CancellationToken>()))
					.Returns(Task.CompletedTask)
					.Verifiable())
			.BuildServiceProvider();
		var coordinator = sp.GetRequiredService<IStartupActionCoordinator>();
		await coordinator.PerformStartupActionsAsync(default);

		var startupActionMock = sp.GetRequiredService<Mock<IAsyncStartupAction>>();
		startupActionMock.Verify(x => x.PerformActionAsync(It.IsAny<CancellationToken>()), Times.Once());
	}
}