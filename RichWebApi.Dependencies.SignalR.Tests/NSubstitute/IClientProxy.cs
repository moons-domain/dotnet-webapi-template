using Microsoft.AspNetCore.SignalR;

namespace RichWebApi.Tests.NSubstitute;

public interface IClientProxy<THub> : IClientProxy where THub : Hub
{

}