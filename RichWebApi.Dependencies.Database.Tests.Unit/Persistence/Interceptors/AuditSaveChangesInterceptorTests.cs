using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using RichWebApi.Persistence.Interceptors;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Entities;
using RichWebApi.Tests.Logging;
using RichWebApi.Tests.Moq;
using Xunit.Abstractions;

namespace RichWebApi.Tests.Persistence.Interceptors;

public class AuditSaveChangesInterceptorTests : UnitTest
{
	private readonly DependencyContainerFixture _container;

	public AuditSaveChangesInterceptorTests(ITestOutputHelper testOutputHelper,
											UnitDependencyContainerFixture container) : base(testOutputHelper)
	{
		var parts = new AppPartsCollection
		{
			new DatabaseUnitTestsPart()
		};
		_container = container
			.WithXunitLogging(testOutputHelper)
			.ReplaceWithEmptyMock<ILoggingOptions>()
			.WithTestScopeInMemoryDatabase(parts);
	}

	[Fact]
	public async Task DontDoAnythingIfUnknownDbContext()
	{
		var now = DateTimeOffset.UtcNow;
		var sp = _container
			.ReplaceWithMock<ISystemClock>(mock => SetupClockMock(mock, now))
			.BuildServiceProvider();
		var clockMock = sp.GetRequiredService<Mock<ISystemClock>>();
		var eventData = new UnitTestDbContextEventData(sp.GetRequiredService<ILoggingOptions>(), null);

		var interceptor = new AuditSaveChangesInterceptor(sp.GetRequiredService<ILogger<AuditSaveChangesInterceptor>>(),
			clockMock.Object);
		await interceptor.SavingChangesAsync(eventData, default);
		clockMock.Verify(x => x.UtcNow, Times.Never());
	}

	[Fact]
	public async Task AuditsCreatedAuditableEntity()
	{
		var now = DateTimeOffset.UtcNow;
		var sp = _container
			.ReplaceWithMock<ISystemClock>(mock => SetupClockMock(mock, now))
			.BuildServiceProvider();
		var clockMock = sp.GetRequiredService<Mock<ISystemClock>>();
		var entity = new UnitAuditableEntity();
		var dbContext = sp.GetRequiredService<RichWebApiDbContext>();
		var eventData = new UnitTestDbContextEventData(sp.GetRequiredService<ILoggingOptions>(), dbContext);

		await dbContext.AddAsync(entity);
		var interceptor = new AuditSaveChangesInterceptor(sp.GetRequiredService<ILogger<AuditSaveChangesInterceptor>>(),
			clockMock.Object);
		await interceptor.SavingChangesAsync(eventData, default);
		clockMock.Verify(x => x.UtcNow, Times.Once());
		entity.Should().BeEquivalentTo(new UnitAuditableEntity
		{
			Id = 1,
			CreatedAt = now.DateTime,
			ModifiedAt = now.DateTime,
			DeletedAt = null
		});
	}

	[Fact]
	public async Task AuditsModifiedAuditableEntity()
	{
		var now = DateTimeOffset.UtcNow;
		var sp = _container
			.ReplaceWithMock<ISystemClock>(mock => SetupClockMock(mock, now))
			.BuildServiceProvider();
		var clockMock = sp.GetRequiredService<Mock<ISystemClock>>();
		var entity = new UnitAuditableEntity();
		var dbContext = sp.GetRequiredService<RichWebApiDbContext>();
		var eventData = new UnitTestDbContextEventData(sp.GetRequiredService<ILoggingOptions>(), dbContext);

		var entry = await dbContext.AddAsync(entity);
		var interceptor = new AuditSaveChangesInterceptor(sp.GetRequiredService<ILogger<AuditSaveChangesInterceptor>>(),
			clockMock.Object);
		await interceptor.SavingChangesAsync(eventData, default);

		var aBitLater = now.AddDays(1);
		clockMock.Setup(x => x.UtcNow)
			.Returns(aBitLater);
		entry.State = EntityState.Modified;

		await interceptor.SavingChangesAsync(eventData, default);
		entity.Should().BeEquivalentTo(new UnitAuditableEntity
		{
			Id = 1,
			CreatedAt = now.DateTime,
			ModifiedAt = aBitLater.DateTime,
			DeletedAt = null
		});
		clockMock.Verify(x => x.UtcNow, Times.Exactly(2));
	}

	[Fact]
	public async Task AuditsSoftDeletedEntity()
	{
		var now = DateTimeOffset.UtcNow;
		var sp = _container
			.ReplaceWithMock<ISystemClock>(mock => SetupClockMock(mock, now))
			.BuildServiceProvider();
		var clockMock = sp.GetRequiredService<Mock<ISystemClock>>();
		var entity = new UnitAuditableEntity();
		var dbContext = sp.GetRequiredService<RichWebApiDbContext>();
		var eventData = new UnitTestDbContextEventData(sp.GetRequiredService<ILoggingOptions>(), dbContext);

		var entry = await dbContext.AddAsync(entity);
		var interceptor = new AuditSaveChangesInterceptor(sp.GetRequiredService<ILogger<AuditSaveChangesInterceptor>>(),
			clockMock.Object);

		await interceptor.SavingChangesAsync(eventData, default);
		entry.State = EntityState.Deleted;

		await interceptor.SavingChangesAsync(eventData, default);

		entity.Should().BeEquivalentTo(new UnitAuditableEntity
		{
			Id = 1,
			CreatedAt = now.DateTime,
			ModifiedAt = now.DateTime,
			DeletedAt = now.DateTime
		});
		clockMock.Verify(x => x.UtcNow, Times.Exactly(2));
	}

	private static void SetupClockMock(Mock<ISystemClock> mock, DateTimeOffset now)
		=> mock.Setup(x => x.UtcNow)
			.Returns(now);
}