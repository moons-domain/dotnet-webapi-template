using RichWebApi.Entities;
using RichWebApi.Persistence;

namespace RichWebApi.Tests;

public static class RichWebApiDatabaseExtensions
{
	public static async ValueTask PersistEntitiesAsync<T>(this IRichWebApiDatabase db, IEnumerable<T> entities, CancellationToken cancellationToken = default)
		where T : class, IEntity
	{
		foreach (var e in entities)
		{
			await db.Context.AddAsync(e, cancellationToken);
		}

		await db.PersistAsync(cancellationToken);
	}

	public static async ValueTask PersistEntityAsync<T>(this IRichWebApiDatabase db, T entity, CancellationToken cancellationToken = default) where T : class, IEntity
	{
		await db.Context.AddAsync(entity, cancellationToken);
		await db.PersistAsync(cancellationToken);
	}
}