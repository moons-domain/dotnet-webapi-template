using FluentAssertions;
using FluentAssertions.Specialized;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RichWebApi.Maintenance;
using RichWebApi.Maintenance.Exceptions;
using RichWebApi.Tests;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Logging;
using RichWebApi.Tests.Moq;
using Xunit.Abstractions;

namespace RichWebApi.Core.Tests.Unit.Maintenance;

public class ApplicationMaintenanceExtensionsTests : UnitTest
{
	private readonly DependencyContainerFixture _container;

	public ApplicationMaintenanceExtensionsTests(ITestOutputHelper testOutputHelper,
												 UnitDependencyContainerFixture container) : base(testOutputHelper)
		=> _container = container
			.WithXunitLogging(TestOutputHelper);

	[Fact]
	public void CallsActionInSyncScope()
	{
		var sp = _container
			.ReplaceWithEmptyMock<Action>()
			.BuildServiceProvider();
		var maintenance = sp.GetRequiredService<ApplicationMaintenance>();
		var actionMock = sp.GetRequiredService<Action>();
		maintenance.ExecuteInScope(actionMock, new MaintenanceReason("unit test"));

		actionMock.Received(1).Invoke();
	}

	[Fact]
	public void CreatesSyncScope()
	{
		var sp = _container
			.BuildServiceProvider();
		var maintenance = sp.GetRequiredService<ApplicationMaintenance>();
		var reason = new MaintenanceReason("unit test");
		maintenance.ExecuteInScope(() =>
		{
			maintenance.IsEnabled
				.Should()
				.BeTrue();

			maintenance.Info
				.Should()
				.NotBeNull()
				.And.BeOfType<MaintenanceInfo>()
				.Which.Reason
				.Should()
				.Be(reason);
		}, reason);

		maintenance.IsEnabled.Should().BeFalse();
	}

	[Fact]
	public void ThrowsIfSyncActionFailed()
	{
		var sp = _container
			.BuildServiceProvider();
		var maintenance = sp.GetRequiredService<ApplicationMaintenance>();
		var reason = new MaintenanceReason("unit test");

		var action = () => maintenance.ExecuteInScope(() => throw new InvalidOperationException(), reason);

		var exceptionAssert = action.Should().ThrowExactly<MaintenanceScopeFailedException>();
		exceptionAssert.WithInnerExceptionExactly<InvalidOperationException>();
		AssertMaintenanceScopeFailure(maintenance, reason, exceptionAssert);
	}

	[Fact]
	public async Task CallsActionInAsyncScope()
	{
		var sp = _container
			.ReplaceWithEmptyMock<Func<Task>>()
			.BuildServiceProvider();
		var maintenance = sp.GetRequiredService<ApplicationMaintenance>();
		var actionMock = sp.GetRequiredService<Func<Task>>();
		await maintenance.ExecuteInScopeAsync(actionMock, new MaintenanceReason("unit test"));

		await actionMock.Received(1).Invoke();
	}

	[Fact]
	public async Task CreatesAsyncScope()
	{
		var sp = _container
			.BuildServiceProvider();
		var maintenance = sp.GetRequiredService<ApplicationMaintenance>();
		var reason = new MaintenanceReason("unit test");
		await maintenance.ExecuteInScopeAsync(() =>
		{
			maintenance.IsEnabled
				.Should()
				.BeTrue();

			maintenance.Info
				.Should()
				.NotBeNull()
				.And.BeOfType<MaintenanceInfo>()
				.Which.Reason
				.Should()
				.Be(reason);
			return Task.CompletedTask;
		}, reason);

		maintenance.IsEnabled.Should().BeFalse();
	}

	[Fact]
	public async Task ThrowsIfAsyncActionFailed()
	{
		var sp = _container
			.BuildServiceProvider();
		var maintenance = sp.GetRequiredService<ApplicationMaintenance>();
		var reason = new MaintenanceReason("unit test");

		var asyncAction = () => maintenance.ExecuteInScopeAsync(() => throw new InvalidOperationException(), reason);

		var exceptionAssert = await asyncAction.Should().ThrowExactlyAsync<MaintenanceScopeFailedException>();
		exceptionAssert.WithInnerExceptionExactly<InvalidOperationException>();
		AssertMaintenanceScopeFailure(maintenance, reason, exceptionAssert);
	}

	private static void AssertMaintenanceScopeFailure(ApplicationMaintenance maintenance, MaintenanceReason reason, ExceptionAssertions<MaintenanceScopeFailedException> exceptionAssert)
	{
		exceptionAssert.WithInnerExceptionExactly<InvalidOperationException>();

		var infoAssert = exceptionAssert.And.Info;
		infoAssert.Should().NotBeNull()
			.And.BeOfType<MaintenanceInfo>()
			.Which.Reason
			.Should()
			.Be(reason);

		maintenance.IsEnabled.Should()
			.BeTrue();

		maintenance.Info.Should()
			.NotBeNull()
			.And.BeOfType<MaintenanceInfo>()
			.Which.Reason
			.Should()
			.NotBeNull()
			.And.BeOfType<MaintenanceScopeFailedReason>()
			.Which.Info
			.Should()
			.NotBeNull()
			.And.BeOfType<MaintenanceInfo>()
			.Which.Reason
			.Should().Be(reason);
	}
}