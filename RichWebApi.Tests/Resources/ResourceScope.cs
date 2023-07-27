namespace RichWebApi.Tests.Resources;

internal sealed class ResourceScope : IResourceScope
{
	private readonly ResourceRepositoryFixture _resourceRepository;


	private readonly List<ResourceScope> _innerScopes = new();

	public string Scope { get; }

	public ResourceScope(string scope, ResourceRepositoryFixture resourceRepository)
	{
		_resourceRepository = resourceRepository;
		Scope = scope;
	}

	public IResourceScope CreateScope(string scope)
	{
		var newScope = new ResourceScope(Scope.ConcatScopeString(scope), _resourceRepository);
		_innerScopes.Add(newScope);
		return newScope;
	}

	public Stream GetResourceStream(string nameSubstring)
		=> _resourceRepository.GetResourceStream($"{Scope}.{nameSubstring}");

	public void Dispose()
	{
		foreach (var scope in _innerScopes)
		{
			scope.Dispose();
		}
	}
}