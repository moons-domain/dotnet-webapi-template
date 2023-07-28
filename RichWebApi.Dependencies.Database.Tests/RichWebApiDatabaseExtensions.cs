using RichWebApi.Entities;
using RichWebApi.Persistence;

namespace RichWebApi.Tests;

public static class RichWebApiDatabaseExtensions
{
	public static async ValueTask PersistEntitiesAsync<T>(this IRichWebApiDatabase db, IEnumerable<T> entities)
		where T : class, IEntity
	{
		foreach (var e in entities)
		{
			await db.Context.AddAsync(e);
		}

		await db.PersistAsync();
	}

	public static async ValueTask PersistEntityAsync<T>(this IRichWebApiDatabase db, T entity) where T : class, IEntity
	{
		await db.Context.AddAsync(entity);
		await db.PersistAsync();
	}
}