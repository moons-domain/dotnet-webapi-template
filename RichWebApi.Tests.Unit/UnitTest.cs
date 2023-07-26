using RichWebApi.Tests.Core;
using RichWebApi.Tests.Core.Resources;
using Xunit.Abstractions;

namespace RichWebApi.Tests.Unit;

[Trait("Category", "Unit")]
public abstract class UnitTest : Test
{
	protected UnitTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}
}