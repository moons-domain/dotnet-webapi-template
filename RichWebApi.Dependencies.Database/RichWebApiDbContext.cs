using Microsoft.EntityFrameworkCore;
using RichWebApi.Persistence;

namespace RichWebApi;

public sealed class RichWebApiDbContext : DbContext
{
	private readonly IEnumerable<ISaveChangesReactor> _saveChangesReactors;

	public RichWebApiDbContext(IEnumerable<ISaveChangesReactor> saveChangesReactors)
		=> _saveChangesReactors = saveChangesReactors;

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