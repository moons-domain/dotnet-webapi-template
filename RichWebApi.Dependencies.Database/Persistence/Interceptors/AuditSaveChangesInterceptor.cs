using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using RichWebApi.Entities;
using RichWebApi.Extensions;

namespace RichWebApi.Persistence.Interceptors;

internal class AuditSaveChangesInterceptor(ILogger<AuditSaveChangesInterceptor> logger, ISystemClock clock)
	: SaveChangesInterceptor, IOrderedInterceptor
{
	public uint Order => 0;


	public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
																				InterceptionResult<int> result,
																				CancellationToken cancellationToken = default)
	{
		await base.SavingChangesAsync(eventData, result, cancellationToken);

		var ctx = eventData.Context;
		if (ctx is null)
		{
			logger.LogWarning("Database context is null");
			return result;
		}

		logger.Time(() => AuditEntities(ctx.ChangeTracker), "Audit tracked database context '{DatabaseContextName}' entities",
			ctx.GetType().Name);
		return result;
	}

	public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
		=> throw new NotSupportedException();

	public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
		=> throw new NotSupportedException();

	public override void SaveChangesCanceled(DbContextEventData eventData)
		=> throw new NotSupportedException();

	public override void SaveChangesFailed(DbContextErrorEventData eventData)
		=> throw new NotSupportedException();

	public override InterceptionResult ThrowingConcurrencyException(ConcurrencyExceptionEventData eventData,
																	InterceptionResult result)
		=> throw new NotSupportedException();

	private void AuditEntities(ChangeTracker changeTracker)
	{
		var entries = changeTracker.Entries();

		foreach (var entry in entries)
		{
			var now = clock.UtcNow.DateTime;

			if (entry.Entity is IAuditableEntity auditable)
			{
				switch (entry.State)
				{
					case EntityState.Modified:
						auditable.ModifiedAt = now;
						break;
					case EntityState.Added:
						auditable.CreatedAt = now;
						auditable.ModifiedAt = now;
						break;
					case EntityState.Detached:
					case EntityState.Unchanged:
					case EntityState.Deleted:
					default:
						break;
				}
			}

			if (entry.Entity is ISoftDeletableEntity softDeletable)
			{
				switch (entry.State)
				{
					case EntityState.Deleted:
						entry.State = EntityState.Modified;
						softDeletable.DeletedAt = now;
						break;
					case EntityState.Modified:
					case EntityState.Added:
					case EntityState.Detached:
					case EntityState.Unchanged:
					default:
						break;
				}
			}
		}
	}
}