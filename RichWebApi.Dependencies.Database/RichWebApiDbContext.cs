using RichWebApi.Entities.Configuration;

namespace RichWebApi;

public sealed class RichWebApiDbContext(
	IDatabaseConfigurator databaseConfigurator,
	DbContextOptions<RichWebApiDbContext> options)
	: DbContext(options)
{
	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		databaseConfigurator.OnModelCreating(builder);
	}

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		base.ConfigureConventions(configurationBuilder);
		databaseConfigurator.ConfigureConventions(configurationBuilder, Database);
	}

	public override int SaveChanges(bool acceptAllChangesOnSuccess) => throw new NotSupportedException();

	public override int SaveChanges() => throw new NotSupportedException();
}