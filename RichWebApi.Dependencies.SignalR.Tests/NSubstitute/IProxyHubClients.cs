using Microsoft.AspNetCore.SignalR;

namespace RichWebApi.Tests.NSubstitute;

public interface IProxyHubClients<THub> : IHubClients where THub : Hub
{

}