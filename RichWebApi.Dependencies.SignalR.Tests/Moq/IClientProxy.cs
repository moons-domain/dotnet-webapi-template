using Microsoft.AspNetCore.SignalR;

namespace RichWebApi.Tests.Moq;

public interface IClientProxy<THub> : IClientProxy where THub : Hub
{
	
}