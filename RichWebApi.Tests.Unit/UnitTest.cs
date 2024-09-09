using RichWebApi.Tests.DependencyInjection;
using Xunit.Abstractions;

namespace RichWebApi.Tests;

[Trait("Category", "Unit")]
public abstract class UnitTest(ITestOutputHelper testOutputHelper)
	: Test(testOutputHelper), IClassFixture<UnitDependencyContainerFixture>;