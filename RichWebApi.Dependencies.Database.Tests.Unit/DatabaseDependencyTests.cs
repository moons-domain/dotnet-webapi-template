using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RichWebApi.Entities;
using RichWebApi.Entities.Configuration;
using RichWebApi.Tests;
using RichWebApi.Tests.Logging;
using Xunit.Abstractions;

namespace RichWebApi;

public class DatabaseDependencyTests : UnitTest
{
	private readonly IServiceProvider _serviceProvider;

	public DatabaseDependencyTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container) : base(testOutputHelper)
	{
		var parts = new AppPartsCollection
		{
			new DatabaseUnitTestsPart()
		};
		_serviceProvider = container
			.WithXunitLogging(testOutputHelper)
			.WithTestScopeInMemoryDatabase(parts)
			.BuildServiceProvider();
	}
	
	[Fact]
	public void CollectsIgnoredEntitiesFromAssembly()
		=> _serviceProvider
			.GetServices<INonGenericEntityConfiguration>()
			.Should()
			.ContainItemsAssignableTo<IIgnoredEntityConfiguration>();
	
	[Fact]
	public void CollectsConfigurableEntities()
		=> _serviceProvider
			.GetServices<INonGenericEntityConfiguration>()
			.Should()
			.ContainItemsAssignableTo<IEntityConfiguration<ConfigurableEntity>>();
}