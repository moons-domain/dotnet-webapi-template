using Microsoft.Extensions.DependencyInjection;
using Moq;
using Polly;
using RichWebApi.Entities;
using RichWebApi.Persistence;
using RichWebApi.Tests;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Logging;
using RichWebApi.Tests.Moq;
using RichWebApi.Utilities;
using Xunit.Abstractions;

namespace RichWebApi;

public class RichWebApiDatabaseTests : UnitTest
{
	private readonly DependencyContainerFixture _container;

	public RichWebApiDatabaseTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container) : base(
		testOutputHelper)
	{
		_container = container
			.WithXunitLogging(testOutputHelper);
	}

	[Fact]
	public async Task CallsReadPolicyOnRead()
	{
		var sp = _container
			.ReplaceWithMock<IAsyncPolicy>(configure: (_, mock) =>
				mock.Setup(x
						=> x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<IEntity>>>(),
							It.IsAny<CancellationToken>()))
					.Verifiable())
			.ReplaceWithMock<IDatabasePolicySet>(configure: (sp, mock) =>
				mock.Setup(x => x.DatabaseReadPolicy)
					.Returns(sp.GetRequiredService<IAsyncPolicy>)
					.Verifiable())
			.WithTestScopeInMemoryDatabase(new AppPartsCollection())
			.BuildServiceProvider();
		var policySetMock = sp.GetRequiredService<Mock<IDatabasePolicySet>>();
		var database = new RichWebApiDatabase(sp.GetRequiredService<RichWebApiDbContext>(), policySetMock.Object);
		await database.ReadAsync((db, ct) => Task.FromResult<IEntity>(new ConfigurableEntity()), default);

		var asyncPolicyMock = sp.GetRequiredService<Mock<IAsyncPolicy>>();

		policySetMock.Verify(x => x.DatabaseReadPolicy, Times.Once());
		asyncPolicyMock.Verify(x => x.ExecuteAsync(
				It.IsAny<Func<CancellationToken, Task<IEntity>>>(),
				It.IsAny<CancellationToken>()),
			Times.Once());
	}

	[Fact]
	public async Task CallsWritePolicyOnWrite()
	{
		var sp = _container
			.ReplaceWithMock<IAsyncPolicy>(configure: (_, mock) =>
				mock.Setup(x
						=> x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(),
							It.IsAny<CancellationToken>()))
					.Verifiable())
			.ReplaceWithMock<IDatabasePolicySet>(configure: (sp, mock) =>
				mock.Setup(x => x.DatabaseWritePolicy)
					.Returns(sp.GetRequiredService<IAsyncPolicy>)
					.Verifiable())
			.WithTestScopeInMemoryDatabase(new AppPartsCollection())
			.BuildServiceProvider();
		var policySetMock = sp.GetRequiredService<Mock<IDatabasePolicySet>>();
		var database = new RichWebApiDatabase(sp.GetRequiredService<RichWebApiDbContext>(), policySetMock.Object);
		await database.WriteAsync((_, _) => Task.CompletedTask, default);

		var asyncPolicyMock = sp.GetRequiredService<Mock<IAsyncPolicy>>();

		policySetMock.Verify(x => x.DatabaseWritePolicy, Times.Once());
		asyncPolicyMock.Verify(x => x.ExecuteAsync(
				It.IsAny<Func<CancellationToken, Task>>(),
				It.IsAny<CancellationToken>()),
			Times.Once());
	}
	
	[Fact]
	public async Task CallsWritePolicyOnPersist()
	{
		var sp = _container
			.ReplaceWithMock<IAsyncPolicy>(configure: (_, mock) =>
				mock.Setup(x
						=> x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(),
							It.IsAny<CancellationToken>()))
					.Verifiable())
			.ReplaceWithMock<IDatabasePolicySet>(configure: (sp, mock) =>
				mock.Setup(x => x.DatabaseWritePolicy)
					.Returns(sp.GetRequiredService<IAsyncPolicy>)
					.Verifiable())
			.WithTestScopeInMemoryDatabase(new AppPartsCollection())
			.BuildServiceProvider();
		var policySetMock = sp.GetRequiredService<Mock<IDatabasePolicySet>>();
		var database = new RichWebApiDatabase(sp.GetRequiredService<RichWebApiDbContext>(), policySetMock.Object);
		await database.PersistAsync();

		var asyncPolicyMock = sp.GetRequiredService<Mock<IAsyncPolicy>>();

		policySetMock.Verify(x => x.DatabaseWritePolicy, Times.Once());
		asyncPolicyMock.Verify(x => x.ExecuteAsync(
				It.IsAny<Func<CancellationToken, Task>>(),
				It.IsAny<CancellationToken>()),
			Times.Once());
	}
}