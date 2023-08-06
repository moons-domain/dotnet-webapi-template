using System.Linq.Expressions;
using JetBrains.Annotations;
using RichWebApi.Exceptions;

namespace RichWebApi.Extensions;

public static class QueryableExtensions
{
	[ItemNotNull]
	public static async Task<T> FirstOrExceptionAsync<T>(this IQueryable<T> query,
		CancellationToken cancellationToken,
		string? customEntityName = null)
	{
		if (query is null)
		{
			throw new ArgumentNullException(nameof(query));
		}
		return await query.FirstOrDefaultAsync(cancellationToken)
			   ?? (customEntityName != null
				   ? throw new CouldNotFindEntityException(customEntityName)
				   : throw new CouldNotFindEntityException(typeof(T)));
	}

	[ItemNotNull]
	public static async Task<T> FirstOrExceptionAsync<T>(this IQueryable<T> query,
														 Expression<Func<T, bool>> predicate,
														 CancellationToken cancellationToken,
														 string? customEntityName = null)
	{
		if (query is null)
		{
			throw new ArgumentNullException(nameof(query));
		}

		return await query.FirstOrDefaultAsync(predicate, cancellationToken)
			   ?? (customEntityName != null
				   ? throw new CouldNotFindEntityException(customEntityName)
				   : throw new CouldNotFindEntityException(typeof(T)));
	}

	public static IQueryable<T> MaybeWhere<T>(this IQueryable<T> query, bool condition,
											  Expression<Func<T, bool>> predicate)
	{
		if (query is null)
		{
			throw new ArgumentNullException(nameof(query));
		}

		return condition
			? query.Where(predicate)
			: query;
	}
}