using Microsoft.AspNetCore.SignalR;

namespace RichWebApi.Tests.Moq;

public interface IProxyHubClients<THub> : IHubClients where THub : Hub
{

}