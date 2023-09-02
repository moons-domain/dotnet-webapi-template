using Microsoft.AspNetCore.SignalR;

namespace RichWebApi.Tests.NSubstitute;

public interface IGroupManager<T> : IGroupManager where T : Hub
{
}