using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using RichWebApi.Entities.Configuration;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Logging;
using RichWebApi.Tests.Moq;
using Xunit.Abstractions;

namespace RichWebApi.Tests.Entities.Configuration;

public class DatabaseConfiguratorTests : UnitTest
{
	private readonly DependencyContainerFixture _container;

	public DatabaseConfiguratorTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container) : base(testOutputHelper)
	{
		var parts = new AppPartsCollection
		{
			new DatabaseUnitTestsPart()
		};
		_container = container
			.WithXunitLogging(testOutputHelper)
			.WithTestScopeInMemoryDatabase(parts, b =>
			{
				// as we need to reset model state which impacts on condition whether OnModelCreating will be called or not
				b.EnableServiceProviderCaching(false);
			})
			.ConfigureServices(s => s.RemoveAll<INonGenericEntityConfiguration>());
	}

	[Fact]
	public async Task IgnoresIgnoredConfigurations()
	{
		var sp = _container
			.ReplaceWithMock<INonGenericEntityConfiguration>(mock =>
			{
				((IEntityConfiguration<IgnoredEntity>)mock).Configure(Arg.Any<EntityTypeBuilder<IgnoredEntity>>());
			})
			.ConfigureServices(s => s.TryAddEnumerable(ServiceDescriptor.Singleton<INonGenericEntityConfiguration, UnitAuditableEntity.Configurator>()))
			.BuildServiceProvider();
		await TriggerDbContextConfigurationAsync(sp);
		var mock = sp.GetRequiredService<INonGenericEntityConfiguration>();
		((IEntityConfiguration<IgnoredEntity>)mock)
			.DidNotReceive().Configure(Arg.Any<EntityTypeBuilder<IgnoredEntity>>());
	}

	[Fact]
	public async Task CallsEntityConfiguration()
	{
		var sp = _container
			.ReplaceWithMock<INonGenericEntityConfiguration>(mock => ((IEntityConfiguration<ConfigurableEntity>)mock)
				.When(x => x.Configure(Arg.Any<EntityTypeBuilder<ConfigurableEntity>>()))
				.Do(x => x.Arg<EntityTypeBuilder<ConfigurableEntity>>().HasNoKey()))
			.ConfigureServices(s => s.TryAddEnumerable(ServiceDescriptor.Singleton<INonGenericEntityConfiguration, UnitAuditableEntity.Configurator>()))
			.BuildServiceProvider();
		await TriggerDbContextConfigurationAsync(sp);
		var mock = sp.GetRequiredService<INonGenericEntityConfiguration>();
		((IEntityConfiguration<ConfigurableEntity>)mock)
			.Received(1)
			.Configure(Arg.Any<EntityTypeBuilder<ConfigurableEntity>>());
	}

	private static ValueTask<EntityEntry<UnitAuditableEntity>> TriggerDbContextConfigurationAsync(IServiceProvider sp)
	{
		var dbContext = sp.GetRequiredService<RichWebApiDbContext>();
		return dbContext.AddAsync(new UnitAuditableEntity());
	}

	public override async Task DisposeAsync()
	{
		await base.DisposeAsync();
		_container.Clear();
	}
}