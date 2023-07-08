namespace RichWebApi;

public static class AppDependenciesBuilderExtensions
{
	public static IAppDependenciesBuilder AddSignalR(this IAppDependenciesBuilder builder, Action<ISignalRDependency> configureSignalR)
	{
		var dependency = new SignalRDependency();
		configureSignalR.Invoke(dependency);
		builder.Add(dependency);
		return builder;
	}
}