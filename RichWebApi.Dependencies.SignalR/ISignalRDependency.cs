using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;

namespace RichWebApi;

public interface ISignalRDependency : IAppDependency
{
	ISignalRDependency WithHub<T>(string pattern, Action<HttpConnectionDispatcherOptions>? configureOptions = null,
									Action<HubEndpointConventionBuilder>? configureConventions = null) where T : Hub;
}