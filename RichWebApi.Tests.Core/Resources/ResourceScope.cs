namespace RichWebApi.Tests.Resources;

internal sealed class ResourceScope : IResourceScope
{
	private readonly ResourceRepositoryFixture _resourceRepository;

	private readonly string _scope;

	private readonly List<ResourceScope> _innerScopes = new();

	public ResourceScope(string scope, ResourceRepositoryFixture resourceRepository)
	{
		_resourceRepository = resourceRepository;
		_scope = scope;
	}

	public IResourceScope BeginScope(string scope)
	{
		var newScope = new ResourceScope($"{_scope}.{scope}", _resourceRepository);
		_innerScopes.Add(newScope);
		return newScope;
	}

	public Stream GetResourceStream(string nameSubstring)
		=> _resourceRepository.GetResourceStream($"{_scope}.{nameSubstring}");

	public void Dispose()
	{
		foreach (var scope in _innerScopes)
		{
			scope.Dispose();
		}
	}
}