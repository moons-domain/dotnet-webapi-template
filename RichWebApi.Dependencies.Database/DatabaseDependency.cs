using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RichWebApi.Config;
using RichWebApi.Entities;
using RichWebApi.Entities.Configuration;
using RichWebApi.Persistence;
using RichWebApi.Persistence.Internal;
using RichWebApi.Startup;
using RichWebApi.Utilities;
using RichWebApi.Utilities.Paging;
using RichWebApi.Validation;

namespace RichWebApi;

internal class DatabaseDependency : IAppDependency
{
	private readonly IHostEnvironment _environment;
	private const string ConfigurationSection = "Database";

	public DatabaseDependency(IHostEnvironment environment) => _environment = environment;

	public void ConfigureServices(IServiceCollection services, IAppPartsCollection parts)
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
		services.AddStartupAction<AssertEntityValidatorsAction>();
		services.AddStartupAction<DatabaseMigrationAction>();
		services.AddSaveChangesReactor<AuditSaveChangesReactor>();
		services.AddSaveChangesReactor<ValidationSaveChangesReactor>();
		services.TryAddScoped<IRichWebApiDatabase, RichWebApiDatabase>();
		services.TryAddScoped<IDatabasePolicySet, DatabasePolicySet>();
		services.TryAddScoped<IDatabaseConfigurator, DatabaseConfigurator>();
		services.TryAddScoped<IValidator<IPagedRequest>, IPagedRequest.Validator>();
		services.TryAddScoped<IValidator<IAuditableEntity>, IAuditableEntity.Validator>();
		services.TryAddScoped<IValidator<ISoftDeletableEntity>, ISoftDeletableEntity.Validator>();
		AddInternalServices(services);
		services.CollectDatabaseEntities(parts.Select(x => x.GetType().Assembly));
	}

	private static void AddInternalServices(IServiceCollection services)
		=> services.TryAddSingleton<IEntityValidatorsProvider, EntityValidatorsProvider>();


	public void ConfigureApplication(IApplicationBuilder builder)
	{
	}
}