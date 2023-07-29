using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RichWebApi.Tests.DependencyInjection;

namespace RichWebApi.Tests.Moq;

public static class DependencyContainerFixtureExtensions
{
	public static DependencyContainerFixture WithMockedSignalRHubContext<T>(
		this DependencyContainerFixture fixture,
		MockBehavior defaultMockBehavior = MockBehavior.Default,
		object[]? args = null,
		Action<IServiceProvider, Mock<IHubContext<T>>>? configureHubContext = null,
		Action<IServiceProvider, Mock<IProxyHubClients<T>>, IClientProxy>? configureHubClients = null,
		Action<IServiceProvider, Mock<IGroupManager<T>>>? configureGroupManager = null,
		Action<IServiceProvider, Mock<IClientProxy<T>>>? configureProxy = null)
		where T : Hub
		=> fixture
			.ReplaceWithMock(defaultMockBehavior, args, configureProxy)
			.ReplaceWithMock<IProxyHubClients<T>>(defaultMockBehavior, args, (sp, mock) =>
			{
				var client = sp.GetRequiredService<IClientProxy<T>>();
				mock.Setup(x => x.All)
					.Returns(client);
				configureHubClients?.Invoke(sp, mock, client);
			})
			.ReplaceWithMock(defaultMockBehavior, args, configureGroupManager)
			.ReplaceWithMock<IHubContext<T>>(defaultMockBehavior, args, (sp, mock) =>
			{
				mock.Setup(x => x.Clients)
					.Returns(sp.GetRequiredService<IProxyHubClients<T>>());
				mock.Setup(x => x.Groups)
					.Returns(sp.GetRequiredService<IGroupManager<T>>());
				configureHubContext?.Invoke(sp, mock);
			});

	public static DependencyContainerFixture WithMockedSignalRHubContext<T, TClient>(
		this DependencyContainerFixture fixture,
		MockBehavior defaultMockBehavior = MockBehavior.Default,
		object[]? args = null,
		Action<IServiceProvider, Mock<IHubContext<T, TClient>>>? configureHubContext = null,
		Action<IServiceProvider, Mock<IHubClients<TClient>>, TClient>? configureHubClients = null,
		Action<IServiceProvider, Mock<IGroupManager<T>>>? configureGroupManager = null,
		Action<IServiceProvider, Mock<TClient>>? configureClient = null)
		where T : Hub<TClient>
		where TClient : class
		=> fixture
			.ReplaceWithMock(defaultMockBehavior, args, configureClient)
			.ReplaceWithMock<IHubClients<TClient>>(defaultMockBehavior, args, (sp, mock) =>
			{
				var client = sp.GetRequiredService<TClient>();
				mock.Setup(x => x.All)
					.Returns(client);
				configureHubClients?.Invoke(sp, mock, client);
			})
			.ReplaceWithMock(defaultMockBehavior, args, configureGroupManager)
			.ReplaceWithMock<IHubContext<T, TClient>>(defaultMockBehavior, args, (sp, mock) =>
			{
				mock.Setup(x => x.Clients)
					.Returns(sp.GetRequiredService<IHubClients<TClient>>());
				mock.Setup(x => x.Groups)
					.Returns(sp.GetRequiredService<IGroupManager<T>>());
				configureHubContext?.Invoke(sp, mock);
			});
}