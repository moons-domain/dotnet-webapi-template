using Microsoft.Extensions.Hosting;

namespace RichWebApi;

public static class AppDependenciesCollectionExtensions
{
	public static IAppDependenciesCollection AddDatabase(this IAppDependenciesCollection dependencies, IHostEnvironment hostEnvironment)
	{
		dependencies.Add(new DatabaseDependency(hostEnvironment));
		return dependencies;
	}
}