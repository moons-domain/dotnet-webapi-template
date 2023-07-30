using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using RichWebApi.Tests;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Logging;
using RichWebApi.Tests.Moq;
using Xunit.Abstractions;

namespace RichWebApi.Entities.Configuration;

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
			.ReplaceWithMock<INonGenericEntityConfiguration>(configure: mock => mock
				.As<IIgnoredEntityConfiguration>()
				.As<IEntityConfiguration<IgnoredEntity>>()
				.Setup(x => x.Configure(It.IsAny<EntityTypeBuilder<IgnoredEntity>>()))
				.Verifiable())
			.ConfigureServices(s => s.TryAddEnumerable(ServiceDescriptor.Singleton<INonGenericEntityConfiguration, UnitAuditableEntity.Configurator>()))
			.BuildServiceProvider();
		await TriggerDbContextConfigurationAsync(sp);
		var mock = sp.GetRequiredService<Mock<INonGenericEntityConfiguration>>();
		mock
			.As<IEntityConfiguration<IgnoredEntity>>()
			.Verify(x => x.Configure(It.IsAny<EntityTypeBuilder<IgnoredEntity>>()), Times.Never());
	}
	
	[Fact]
	public async Task CallsEntityConfiguration()
	{
		var sp = _container
			.ReplaceWithMock<INonGenericEntityConfiguration>(configure: mock => mock
				.As<IEntityConfiguration<ConfigurableEntity>>()
				.Setup(x => x.Configure(It.IsAny<EntityTypeBuilder<ConfigurableEntity>>()))
				.Callback<EntityTypeBuilder<ConfigurableEntity>>(b => b.HasNoKey())
				.Verifiable())
			.ConfigureServices(s => s.TryAddEnumerable(ServiceDescriptor.Singleton<INonGenericEntityConfiguration, UnitAuditableEntity.Configurator>()))
			.BuildServiceProvider();
		await TriggerDbContextConfigurationAsync(sp);
		var mock = sp.GetRequiredService<Mock<INonGenericEntityConfiguration>>();
		mock
			.As<IEntityConfiguration<ConfigurableEntity>>()
			.Verify(x => x.Configure(It.IsAny<EntityTypeBuilder<ConfigurableEntity>>()), Times.Once());
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