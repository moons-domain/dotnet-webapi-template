using JetBrains.Annotations;

namespace RichWebApi.Utilities.Paging;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public sealed class PagedResult<T>
{
	public PagedResult(IReadOnlyCollection<T> items, int page, int size, int total)
	{
		Items = items;
		Page = page;
		Size = size;
		Total = total;
	}

	public IReadOnlyCollection<T> Items { get; }

	public int Page { get; }

	public int Size { get; }

	public int Total { get; }
}