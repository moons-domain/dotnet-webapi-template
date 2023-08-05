using FluentAssertions;
using RichWebApi.Maintenance;
using RichWebApi.Tests;
using Xunit.Abstractions;

namespace RichWebApi.Core.Tests.Unit.Maintenance;

public class MaintenanceReasonTests : UnitTest
{
	public MaintenanceReasonTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public void ThrowsIfInvalidDescriptionTheory(string? description)
	{
		var action = () => new MaintenanceReason(description!);
		action.Should()
			.ThrowExactly<ArgumentException>()
			.And.ParamName
			.Should()
			.Be("description");
	}
}