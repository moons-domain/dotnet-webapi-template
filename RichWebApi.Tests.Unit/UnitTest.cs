using Xunit.Abstractions;

namespace RichWebApi.Tests;

[Trait("Category", "Unit")]
public abstract class UnitTest : Test
{
	protected UnitTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}
}