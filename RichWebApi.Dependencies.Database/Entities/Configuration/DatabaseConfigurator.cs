using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using RichWebApi.Extensions;

namespace RichWebApi.Entities.Configuration;

internal class DatabaseConfigurator(
	IEnumerable<INonGenericEntityConfiguration> configurations,
	ILogger<DatabaseConfigurator> logger)
	: IDatabaseConfigurator
{
	public void OnModelCreating(ModelBuilder modelBuilder) => logger.Time(() =>
	{
		foreach (var configuration in configurations)
		{
			if (configuration is IIgnoredEntityConfiguration)
			{
				continue;
			}

			try
			{
				ConfigureEntity(modelBuilder, configuration.GetType(), configuration);
			}
			catch (Exception e)
			{
				logger.LogError(e, "Error when configuring db entities");
				throw;
			}
		}
	}, $"{nameof(OnModelCreating)}({{Count}})", configurations.Count());

	public void ConfigureConventions(ModelConfigurationBuilder configurationBuilder, DatabaseFacade databaseFacade)
	{
		if (databaseFacade.IsSqlServer())
		{
			// apply conventions for sql server here
		}
	}

	private void ConfigureEntity(ModelBuilder modelBuilder,
								 Type configurationType,
								 INonGenericEntityConfiguration configuration)
	{
		var configurationGenericInterface = configurationType
			.GetInterface(typeof(IEntityTypeConfiguration<>).Name);

		if (configurationGenericInterface is null)
		{
			throw new InvalidOperationException(string.Format(
				"Type {0} marked with {1} does not implement ${2}",
				configurationType,
				nameof(INonGenericEntityConfiguration),
				typeof(IEntityTypeConfiguration<>).Name));
		}

		var configuredType = configurationGenericInterface.GetGenericArguments()[0];
		var modelBuilderEntity = modelBuilder
			.GetType()
			.GetMethods()
			.First(x => x is { Name: nameof(ModelBuilder.Entity), IsGenericMethodDefinition: true })
			.MakeGenericMethod(configuredType);

		var entityTypeBuilder = modelBuilderEntity.Invoke(modelBuilder, null);

		var configureMethod =
			configurationType.GetMethod(nameof(IEntityTypeConfiguration<object>.Configure));

		if (configureMethod is null)
		{
			throw new InvalidOperationException(string.Format(
				"Type {0} doesn't contain Configure method from {1} interface",
				configurationType,
				configurationGenericInterface.Name));
		}

		configureMethod.Invoke(configuration, [entityTypeBuilder]);
	}
}