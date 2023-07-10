namespace RichWebApi;

public class AppDependenciesBuilder : IAppDependenciesBuilder
{
	private readonly IDictionary<Type, IAppDependency> _dependencies = new Dictionary<Type, IAppDependency>();
	public IAppDependenciesBuilder Add(IAppDependency dependency)
	{
		_dependencies.TryAdd(dependency.GetType(), dependency);
		return this;
	}

	public IReadOnlyCollection<IAppDependency> Build()
		=> _dependencies.Values.ToList().AsReadOnly();
}