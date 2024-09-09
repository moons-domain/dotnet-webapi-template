using RichWebApi.Tests.Resources;
using Xunit.Abstractions;

namespace RichWebApi.Tests;

public abstract class Test(ITestOutputHelper testOutputHelper)
	: IClassFixture<ResourceRepositoryFixture>, IAsyncLifetime
{
	protected ITestOutputHelper TestOutputHelper { get; } = testOutputHelper;

	public virtual Task InitializeAsync() => Task.CompletedTask;

	public virtual Task DisposeAsync() => Task.CompletedTask;
}