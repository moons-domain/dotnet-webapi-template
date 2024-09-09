namespace RichWebApi.Tests.Resources;

internal sealed class ResourceScope(string scope, ResourceRepositoryFixture resourceRepository) : IResourceScope
{
	private readonly List<ResourceScope> _innerScopes = [];

	public string Scope { get; } = scope;

	public IResourceScope CreateScope(string scope)
	{
		var newScope = new ResourceScope(Scope.ConcatScopeString(scope), resourceRepository);
		_innerScopes.Add(newScope);
		return newScope;
	}

	public Stream GetResourceStream(string nameSubstring)
		=> resourceRepository.GetResourceStream($"{Scope}.{nameSubstring}");

	public void Dispose()
	{
		foreach (var scope in _innerScopes)
		{
			scope.Dispose();
		}
	}
}