using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using RichWebApi.Config;
using RichWebApi.Entities.Configuration;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Entities;
using RichWebApi.Tests.Logging;
using RichWebApi.Tests.NSubstitute;
using Xunit.Abstractions;

namespace RichWebApi.Tests;

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
		var envMock = sp.GetRequiredService<IHostEnvironment>();
		var dependency = new DatabaseDependency(envMock);
		dependency.ConfigureServices(services, parts);
		var dependencyServiceProvider = services.BuildServiceProvider();
		dependencyServiceProvider.GetRequiredService<IValidator<DatabaseConfig>>()
			.Should()
			.BeOfType<DatabaseConfig.DevEnvValidator>();
		var _ = envMock.Received(1).EnvironmentName;
	}

	[Fact]
	public void UsesProdConfigValidatorInNonDevEnvironment()
	{
		var services = new ServiceCollection();
		var parts = new AppPartsCollection();
		var sp = SetEnvironment(_container, Environments.Production)
			.BuildServiceProvider();
		var envMock = sp.GetRequiredService<IHostEnvironment>();
		var dependency = new DatabaseDependency(envMock);
		dependency.ConfigureServices(services, parts);
		var dependencyServiceProvider = services.BuildServiceProvider();
		dependencyServiceProvider.GetRequiredService<IValidator<DatabaseConfig>>()
			.Should()
			.BeOfType<DatabaseConfig.ProdEnvValidator>();
		var _ = envMock.Received(1).EnvironmentName;
	}

	private static DependencyContainerFixture SetEnvironment(DependencyContainerFixture container, string environmentName)
		=> container
			.ReplaceWithMock<IHostEnvironment>(mock => mock.EnvironmentName
				.Returns(environmentName));
}