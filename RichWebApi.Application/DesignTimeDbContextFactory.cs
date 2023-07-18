using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.FileProviders;
using RichWebApi.Parts;

namespace RichWebApi;

[UsedImplicitly]
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RichWebApiDbContext>
{
	private sealed class DummyEnvironment : IWebHostEnvironment
	{
		public string ApplicationName { get; set; } = null!;
		public IFileProvider ContentRootFileProvider { get; set; } = null!;
		public string ContentRootPath { get; set; } = null!;
		public string EnvironmentName { get; set; } = Environments.Development;
		public string WebRootPath { get; set; } = null!;
		public IFileProvider WebRootFileProvider { get; set; } = null!;
	}

	public RichWebApiDbContext CreateDbContext(string[] args)
	{
		var configurationRoot = new ConfigurationBuilder()
			.AddUserSecrets<Program>()
			.AddEnvironmentVariables()
			.Build();
		var env = new DummyEnvironment();
		var dependencies = new AppDependenciesCollection()
			.AddDatabase(env);
		var provider = new ServiceCollection()
			.AddLogging(x =>
			{
				x.SetMinimumLevel(LogLevel.Information);
				x.AddConsole();
			})
			.AddCore()
			.AddSingleton<IConfiguration>(configurationRoot)
			.AddSingleton(configurationRoot)
			.AddSingleton<IWebHostEnvironment>(env)
			.AddDependencyServices(dependencies)
			.EnrichWithApplicationParts(new AppPartsCollection().AddWeather())
			.BuildServiceProvider();
		return provider.GetRequiredService<RichWebApiDbContext>();
	}
}