using Microsoft.AspNetCore.SignalR;

namespace RichWebApi.Tests.Moq;

public interface IGroupManager<T> : IGroupManager where T : Hub
{
}