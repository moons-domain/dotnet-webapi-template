using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RichWebApi.Maintenance;
using RichWebApi.Maintenance.Exceptions;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Logging;
using Xunit.Abstractions;

namespace RichWebApi.Tests.Maintenance;

public class ApplicationMaintenanceTests : UnitTest
{
	private readonly IServiceProvider _serviceProvider;

	public ApplicationMaintenanceTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container) : base(testOutputHelper)
		=> _serviceProvider = container
		.WithXunitLogging(TestOutputHelper)
		.BuildServiceProvider();

	[Fact]
	public void EnablesMaintenance()
	{
		var maintenance = _serviceProvider.GetRequiredService<ApplicationMaintenance>();

		var reason = new MaintenanceReason("unit test");
		maintenance.Enable(reason);

		maintenance.IsEnabled.Should().BeTrue();
		maintenance.Info
			.Should()
			.NotBeNull();

		maintenance.Info.Reason
			.Should()
			.Be(reason);
	}

	[Fact]
	public void InfoThrowsIfDisabled()
	{
		var maintenance = _serviceProvider.GetRequiredService<ApplicationMaintenance>();

		var action = () => maintenance.Info;

		action
			.Should()
			.ThrowExactly<InvalidOperationException>();
	}

	[Fact]
	public void EnableThrowsIfAlreadyEnabled()
	{
		var maintenance = _serviceProvider.GetRequiredService<ApplicationMaintenance>();
		maintenance.Enable(new MaintenanceReason("unit test"));

		var action = () => maintenance.Enable(new MaintenanceReason("unit test"));

		action.Should()
			.ThrowExactly<MaintenanceAlreadyEnabledException>()
			.And.Info.Should()
			.Be(maintenance.Info);
	}

	[Fact]
	public void DisableThrowsIfAlreadyDisabled()
	{
		var maintenance = _serviceProvider.GetRequiredService<ApplicationMaintenance>();

		var action = () => maintenance.Disable();

		action.Should().ThrowExactly<MaintenanceAlreadyDisabledException>();
	}
}