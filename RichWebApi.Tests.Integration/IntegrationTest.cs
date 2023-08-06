using RichWebApi.Tests.DependencyInjection;
using Xunit.Abstractions;

namespace RichWebApi.Tests;

[Trait("Category", "Integration")]
public abstract class IntegrationTest : Test, IClassFixture<IntegrationDependencyContainerFixture>
{
	protected IntegrationTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}
}