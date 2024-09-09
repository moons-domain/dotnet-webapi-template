using RichWebApi.Tests.DependencyInjection;
using Xunit.Abstractions;

namespace RichWebApi.Tests;

[Trait("Category", "Integration")]
public abstract class IntegrationTest(ITestOutputHelper testOutputHelper)
	: Test(testOutputHelper), IClassFixture<IntegrationDependencyContainerFixture>;