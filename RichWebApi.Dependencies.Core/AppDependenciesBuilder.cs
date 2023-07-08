namespace RichWebApi;

public class AppDependenciesBuilder : IAppDependenciesBuilder
{
	private readonly List<IAppDependency> _dependencies = new();
	public IAppDependenciesBuilder Add(IAppDependency dependency)
	{
		_dependencies.Add(dependency);
		return this;
	}

	public IReadOnlyCollection<IAppDependency> Build()
		=> _dependencies.AsReadOnly();
}