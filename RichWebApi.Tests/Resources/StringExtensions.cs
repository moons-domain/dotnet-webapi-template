namespace RichWebApi.Tests.Resources;

internal static class StringExtensions
{
	public static string ConcatScopeString(this string scope, params string[] innerScopes)
	{
		if (innerScopes.Length == 0)
		{
			return scope;
		}

		const char separator = '.';
		return $"{scope}{separator}{string.Join(separator, innerScopes)}";
	}
}