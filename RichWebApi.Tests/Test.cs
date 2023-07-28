using RichWebApi.Tests.Resources;
using Xunit.Abstractions;

namespace RichWebApi.Tests;

public abstract class Test : IClassFixture<ResourceRepositoryFixture>, IAsyncLifetime
{
	protected ITestOutputHelper TestOutputHelper { get; }

	protected Test(ITestOutputHelper testOutputHelper) 
		=> TestOutputHelper = testOutputHelper;

	public virtual Task InitializeAsync() => Task.CompletedTask;

	public virtual Task DisposeAsync() => Task.CompletedTask;
}