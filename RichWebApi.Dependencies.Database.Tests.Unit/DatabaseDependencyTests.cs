using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using RichWebApi.Config;
using RichWebApi.Entities;
using RichWebApi.Entities.Configuration;
using RichWebApi.Tests;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Logging;
using RichWebApi.Tests.Moq;
using Xunit.Abstractions;

namespace RichWebApi;

public class DatabaseDependencyTests : UnitTest
{
	private readonly DependencyContainerFixture _container;

	public DatabaseDependencyTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container) : base(testOutputHelper)
		=> _container = container
		.WithXunitLogging(TestOutputHelper);

	[Fact]
	public void CollectsIgnoredEntitiesFromAssembly()
		=> _container
			.WithTestScopeInMemoryDatabase(new AppPartsCollection
			{
				new DatabaseUnitTestsPart()
			})
			.BuildServiceProvider()
			.GetServices<INonGenericEntityConfiguration>()
			.Should()
			.ContainItemsAssignableTo<IIgnoredEntityConfiguration>();

	[Fact]
	public void CollectsConfigurableEntities()
		=> _container
			.WithTestScopeInMemoryDatabase(new AppPartsCollection
			{
				new DatabaseUnitTestsPart()
			})
			.BuildServiceProvider()
			.GetServices<INonGenericEntityConfiguration>()
			.Should()
			.ContainItemsAssignableTo<IEntityConfiguration<ConfigurableEntity>>();

	[Fact]
	public void UsesDevConfigValidatorInDevEnvironment()
	{
		var services = new ServiceCollection();
		var parts = new AppPartsCollection();
		var sp = SetEnvironment(_container, Environments.Development)
			.BuildServiceProvider();
		var envMock = sp.GetRequiredService<Mock<IHostEnvironment>>();
		var dependency = new DatabaseDependency(envMock.Object);
		dependency.ConfigureServices(services, parts);
		var dependencyServiceProvider = services.BuildServiceProvider();
		dependencyServiceProvider.GetRequiredService<IValidator<DatabaseConfig>>()
			.Should()
			.BeOfType<DatabaseConfig.DevEnvValidator>();
		envMock.Verify(x => x.EnvironmentName, Times.Once());
	}

	[Fact]
	public void UsesProdConfigValidatorInNonDevEnvironment()
	{
		var services = new ServiceCollection();
		var parts = new AppPartsCollection();
		var sp = SetEnvironment(_container, Environments.Production)
			.BuildServiceProvider();
		var envMock = sp.GetRequiredService<Mock<IHostEnvironment>>();
		var dependency = new DatabaseDependency(envMock.Object);
		dependency.ConfigureServices(services, parts);
		var dependencyServiceProvider = services.BuildServiceProvider();
		dependencyServiceProvider.GetRequiredService<IValidator<DatabaseConfig>>()
			.Should()
			.BeOfType<DatabaseConfig.ProdEnvValidator>();
		envMock.Verify(x => x.EnvironmentName, Times.Once());
	}

	private static DependencyContainerFixture SetEnvironment(DependencyContainerFixture container, string environmentName)
		=> container
			.ReplaceWithMock<IHostEnvironment>(mock => mock
				.Setup(x => x.EnvironmentName)
				.Returns(environmentName)
				.Verifiable());
}