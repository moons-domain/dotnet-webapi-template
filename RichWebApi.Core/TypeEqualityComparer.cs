namespace RichWebApi;

public class TypeEqualityComparer<T> : IEqualityComparer<T>
{
	public bool Equals(T? x, T? y)
	{
		if (x is null && y is null)
		{
			return true;
		}

		return x?.GetType() == y?.GetType();
	}

	public int GetHashCode(T obj) => obj is null
		? 0
		: obj.GetHashCode();
}