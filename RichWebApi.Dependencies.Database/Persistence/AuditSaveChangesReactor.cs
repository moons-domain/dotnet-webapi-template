using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using RichWebApi.Entities;
using RichWebApi.Extensions;

namespace RichWebApi.Persistence;

[UsedImplicitly]
public class AuditSaveChangesReactor : ISaveChangesReactor
{
	private readonly ILogger<AuditSaveChangesReactor> _logger;
	private readonly ISystemClock _clock;

	public AuditSaveChangesReactor(ILogger<AuditSaveChangesReactor> logger, ISystemClock clock)
	{
		_logger = logger;
		_clock = clock;
	}

	public uint Order => 0;

	public ValueTask ReactAsync(RichWebApiDbContext context, CancellationToken cancellationToken)
	{
		_logger.Time(() => OnBeforeSaving(context), "Audit tracked database entities");
		return new ValueTask();
	}

	private void OnBeforeSaving(DbContext context)
	{
		var entries = context.ChangeTracker.Entries();

		foreach (var entry in entries)
		{
			var now = _clock.UtcNow.DateTime;

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