namespace RichWebApi;

public interface IAppDependenciesBuilder
{
	public IAppDependenciesBuilder Add(IAppDependency dependency);

	public IReadOnlyCollection<IAppDependency> Build();
}