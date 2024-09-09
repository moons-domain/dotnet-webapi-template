using JetBrains.Annotations;

namespace RichWebApi.Utilities.Paging;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public sealed class PagedResult<T>(IReadOnlyCollection<T> items, int page, int size, int total)
{
	public IReadOnlyCollection<T> Items { get; } = items;

	public int Page { get; } = page;

	public int Size { get; } = size;

	public int Total { get; } = total;
}