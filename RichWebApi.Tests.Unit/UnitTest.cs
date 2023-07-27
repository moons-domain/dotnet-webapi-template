using Xunit.Abstractions;

namespace RichWebApi.Tests;

[Trait("Category", "Unit")]
public abstract class UnitTest : Test, IClassFixture<UnitDependencyContainerFixture>
{
	protected UnitTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}
}