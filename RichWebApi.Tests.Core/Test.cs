using RichWebApi.Tests.Core.DependencyInjection;
using RichWebApi.Tests.Core.Resources;
using Xunit.Abstractions;

namespace RichWebApi.Tests.Core;

public abstract class Test : IClassFixture<ResourceRepositoryFixture>, IClassFixture<DependencyContainerFixture>
{
	protected ITestOutputHelper TestOutputHelper { get; }

	protected Test(ITestOutputHelper testOutputHelper) 
		=> TestOutputHelper = testOutputHelper;
}