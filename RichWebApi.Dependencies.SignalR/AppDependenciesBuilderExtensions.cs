namespace RichWebApi;

public static class AppDependenciesBuilderExtensions
{
	public static IAppDependenciesBuilder AddSignalR(this IAppDependenciesBuilder builder, Action<ISignalRConfigurator> configureSignalR)
	{
		var dependency = new SignalRDependency();
		configureSignalR.Invoke(dependency);
		builder.Add(dependency);
		return builder;
	}
}