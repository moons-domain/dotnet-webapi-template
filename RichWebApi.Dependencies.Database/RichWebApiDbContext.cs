using Microsoft.EntityFrameworkCore;
using RichWebApi.Entities.Configuration;
using RichWebApi.Persistence;

namespace RichWebApi;

public sealed class RichWebApiDbContext : DbContext
{
	private readonly IEnumerable<ISaveChangesReactor> _saveChangesReactors;
	private readonly IDatabaseConfigurator _databaseConfigurator;

	public RichWebApiDbContext(IEnumerable<ISaveChangesReactor> saveChangesReactors,
							   IDatabaseConfigurator databaseConfigurator,
							   DbContextOptions<RichWebApiDbContext> options) : base(options)
	{
		_saveChangesReactors = saveChangesReactors;
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

	public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
													 CancellationToken cancellationToken = default)
	{
		await EmitSaveChangesEventAsync(cancellationToken);
		return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		await EmitSaveChangesEventAsync(cancellationToken);
		return await base.SaveChangesAsync(cancellationToken);
	}

	private async ValueTask EmitSaveChangesEventAsync(CancellationToken cancellationToken)
	{
		foreach (var reactor in _saveChangesReactors.OrderBy(x => x.Order))
		{
			await reactor.ReactAsync(this, cancellationToken);
		}
	}
}