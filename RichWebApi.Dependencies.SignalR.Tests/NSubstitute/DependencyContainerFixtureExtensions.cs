using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RichWebApi.Tests.DependencyInjection;

namespace RichWebApi.Tests.NSubstitute;

public static class DependencyContainerFixtureExtensions
{
	public static DependencyContainerFixture WithMockedSignalRHubContext<T>(
		this DependencyContainerFixture fixture,
		object[]? args = null,
		Action<IServiceProvider, IHubContext<T>>? configureHubContext = null,
		Action<IServiceProvider, IProxyHubClients<T>, IClientProxy>? configureHubClients = null,
		Action<IServiceProvider, IGroupManager<T>>? configureGroupManager = null,
		Action<IServiceProvider, IClientProxy<T>>? configureProxy = null)
		where T : Hub
		=> fixture
			.ReplaceWithMock(configureProxy, args)
			.ReplaceWithMock<IProxyHubClients<T>>((sp, mock) =>
			{
				var client = sp.GetRequiredService<IClientProxy<T>>();
				mock.All.Returns(client);
				configureHubClients?.Invoke(sp, mock, client);
			}, args)
			.ReplaceWithMock(configureGroupManager, args)
			.ReplaceWithMock<IHubContext<T>>((sp, mock) =>
			{
				mock.Clients
					.Returns(sp.GetRequiredService<IProxyHubClients<T>>());
				mock.Groups
					.Returns(sp.GetRequiredService<IGroupManager<T>>());
				configureHubContext?.Invoke(sp, mock);
			}, args);

	public static DependencyContainerFixture WithMockedSignalRHubContext<T, TClient>(
		this DependencyContainerFixture fixture,
		object[]? args = null,
		Action<IServiceProvider, IHubContext<T, TClient>>? configureHubContext = null,
		Action<IServiceProvider, IHubClients<TClient>, TClient>? configureHubClients = null,
		Action<IServiceProvider, IGroupManager<T>>? configureGroupManager = null,
		Action<IServiceProvider, TClient>? configureClient = null)
		where T : Hub<TClient>
		where TClient : class
		=> fixture
			.ReplaceWithMock(configureClient, args)
			.ReplaceWithMock<IHubClients<TClient>>((sp, mock) =>
			{
				var client = sp.GetRequiredService<TClient>();
				mock.All
					.Returns(client);
				configureHubClients?.Invoke(sp, mock, client);
			}, args)
			.ReplaceWithMock(configureGroupManager, args)
			.ReplaceWithMock<IHubContext<T, TClient>>((sp, mock) =>
			{
				mock.Clients
					.Returns(_ => sp.GetRequiredService<IHubClients<TClient>>());
				mock.Groups
					.Returns(_ => sp.GetRequiredService<IGroupManager<T>>());
				configureHubContext?.Invoke(sp, mock);
			}, args);
}