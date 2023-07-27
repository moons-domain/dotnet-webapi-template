using RichWebApi.Tests.Resources;
using Xunit.Abstractions;

namespace RichWebApi.Tests;

public abstract class Test : IClassFixture<ResourceRepositoryFixture>
{
	protected ITestOutputHelper TestOutputHelper { get; }

	protected Test(ITestOutputHelper testOutputHelper) 
		=> TestOutputHelper = testOutputHelper;
}