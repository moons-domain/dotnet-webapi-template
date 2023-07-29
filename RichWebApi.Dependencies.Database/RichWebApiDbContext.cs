using Microsoft.EntityFrameworkCore;
using RichWebApi.Entities.Configuration;
using RichWebApi.Persistence;

namespace RichWebApi;

public sealed class RichWebApiDbContext : DbContext
{
	private readonly IDatabaseConfigurator _databaseConfigurator;

	public RichWebApiDbContext(IDatabaseConfigurator databaseConfigurator,
	                           DbContextOptions<RichWebApiDbContext> options) : base(options)
	{
		_databaseConfigurator = databaseConfigurator;
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		_databaseConfigurator.OnModelCreating(builder);
	}

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		base.ConfigureConventions(configurationBuilder);
		_databaseConfigurator.ConfigureConventions(configurationBuilder, Database);
	}

	public override int SaveChanges(bool acceptAllChangesOnSuccess) => throw new NotSupportedException();

	public override int SaveChanges() => throw new NotSupportedException();
}