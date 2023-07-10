namespace RichWebApi;

public class TypeEqualityComparer<T> : IEqualityComparer<T>
{
	public bool Equals(T? x, T? y)
		=> x is not null && y is not null && x.GetType() == y.GetType();

	public int GetHashCode(T obj) => obj is null
		? 0
		: obj.GetHashCode();
}