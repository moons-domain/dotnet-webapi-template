using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RichWebApi.Config;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Moq;

namespace RichWebApi.Tests;

public static class DependencyContainerFixtureExtensions
{
	public static DependencyContainerFixture SetDatabaseEntitiesConfig(this DependencyContainerFixture container,
	                                                             EntitiesValidationOption option)
		=> container.ReplaceWithMock<IOptionsMonitor<DatabaseEntitiesConfig>>(configure: (sp, mock) => mock.Setup(x => x.CurrentValue)
			.Returns(new DatabaseEntitiesConfig
			{
				Validation = option
			}));
	
	public static DependencyContainerFixture WithTestScopeInMemoryDatabase(
		this DependencyContainerFixture fixture, IAppPartsCollection partsToScan)
	{
		var dependencies = new AppDependenciesCollection()
			.AddDatabase(new DummyEnvironment());
		return fixture
			.ReplaceWithMock<IOptionsMonitor<DatabaseEntitiesConfig>>(configure: (sp, mock) =>
				mock.Setup(x => x.CurrentValue)
					.Returns(new DatabaseEntitiesConfig
					{
						Validation = EntitiesValidationOption.None
					}))
			.ConfigureServices(services => services
				.AddDbContext<RichWebApiDbContext>(x => x
					.UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
					.EnableDetailedErrors()
					.EnableSensitiveDataLogging(), ServiceLifetime.Transient)
				.AddDependencyServices(dependencies, partsToScan));
	}

	private sealed class DummyEnvironment : IWebHostEnvironment
	{
		public string ApplicationName { get; set; } = null!;

		public IFileProvider ContentRootFileProvider { get; set; } = null!;

		public string ContentRootPath { get; set; } = null!;

		public string EnvironmentName { get; set; } = Environments.Development;

		public string WebRootPath { get; set; } = null!;

		public IFileProvider WebRootFileProvider { get; set; } = null!;
	}
}