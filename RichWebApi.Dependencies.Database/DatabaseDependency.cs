using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RichWebApi.Config;
using RichWebApi.Entities.Configuration;
using RichWebApi.Persistence;
using RichWebApi.Startup;
using RichWebApi.Utilities;
using RichWebApi.Validation;

namespace RichWebApi;

internal class DatabaseDependency : IAppDependency
{
	private readonly IHostEnvironment _environment;
	private const string ConfigurationSection = "Database";

	public DatabaseDependency(IHostEnvironment environment) => _environment = environment;

	public void ConfigureServices(IServiceCollection services)
	{
		if (_environment.IsDevelopment())
		{
			services.AddOptionsWithValidator<DatabaseConfig, DatabaseConfig.DevEnvValidator>(ConfigurationSection);
		}
		else
		{
			services.AddOptionsWithValidator<DatabaseConfig, DatabaseConfig.ProdEnvValidator>(ConfigurationSection);
		}

		var migrationsAssemblyName = $"{typeof(DatabaseDependency).Assembly.GetName().Name}.Migrations";
		services.AddDbContext<RichWebApiDbContext>((sp, dbContextOptionsBuilder) =>
		{
			var host = sp.GetRequiredService<IWebHostEnvironment>();
			var dbConfig = sp.GetRequiredService<IOptionsMonitor<DatabaseConfig>>()
				.CurrentValue;

			if (host.IsDevelopment())
			{
				dbContextOptionsBuilder.EnableSensitiveDataLogging();
			}
			dbContextOptionsBuilder
				.UseSqlServer(host.IsDevelopment()
						? dbConfig.ConnectionString
						: $"Server=tcp:{dbConfig.Host},{dbConfig.Port};Initial Catalog={dbConfig.DbInstanceIdentifier};Persist Security Info=False;User ID={dbConfig.Username};Password={dbConfig.Password};MultipleActiveResultSets=True;",
					builder => builder
						.EnableRetryOnFailure(dbConfig.Retries)
						.CommandTimeout(dbConfig.Timeout)
						.MigrationsAssembly(migrationsAssemblyName));
		}, ServiceLifetime.Transient);
		services.AddStartupAction<DatabaseMigrationAction>();
		services.AddSaveChangesReactor<AuditSaveChangesReactor>();
		services.TryAddScoped<IRichWebApiDatabase, RichWebApiDatabase>();
		services.TryAddScoped<IDatabasePolicySet, DatabasePolicySet>();
		services.TryAddScoped<IDatabaseConfigurator, DatabaseConfigurator>();
	}

	public void ConfigureApplication(IApplicationBuilder builder)
	{
	}
}