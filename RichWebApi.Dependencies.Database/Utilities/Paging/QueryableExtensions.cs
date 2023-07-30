namespace RichWebApi.Utilities.Paging;

public static class QueryableExtensions
{
	public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query, IPagedRequest request, CancellationToken cancellationToken)
	{
		var count = await query.CountAsync(cancellationToken);
		var result = await query
			.Skip(request.Size * request.Page)
			.Take(request.Size)
			.ToListAsync(cancellationToken);
		return new PagedResult<T>(result.AsReadOnly(), request.Page, request.Size, count);
	}
}