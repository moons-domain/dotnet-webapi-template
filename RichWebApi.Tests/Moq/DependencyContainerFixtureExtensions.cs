using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using RichWebApi.Tests.DependencyInjection;

namespace RichWebApi.Tests.Moq;

public static class DependencyContainerFixtureExtensions
{
	public static DependencyContainerFixture AddMoqServices(this DependencyContainerFixture fixture,
	                                                        MockBehavior defaultMockBehavior = MockBehavior.Default)
		=> fixture.ConfigureServices(services => services.TryAddSingleton(new MockRepository(defaultMockBehavior)));

	public static DependencyContainerFixture ReplaceWithMock<TService>(
		this DependencyContainerFixture fixture,
		MockBehavior defaultMockBehavior = MockBehavior.Default,
		object[]? args = null,
		Action<IServiceProvider, Mock<TService>>? configure = null) where TService : class
		=> fixture
			.AddMoqServices()
			.ConfigureServices(services =>
			{
				services.Replace(ServiceDescriptor.Singleton(sp =>
				{
					var mockRepository = sp.GetRequiredService<MockRepository>();
					var mock = mockRepository.Create<TService>(defaultMockBehavior, args ?? Array.Empty<object>());
					configure?.Invoke(sp, mock);
					return mock;
				}));
				services.Replace(
					ServiceDescriptor.Singleton(sp => sp.GetRequiredService<Mock<TService>>().Object));
			});

	public static DependencyContainerFixture ReplaceWithMock<TService>(
		this DependencyContainerFixture fixture,
		MockBehavior defaultMockBehavior = MockBehavior.Default,
		object[]? args = null,
		Action<Mock<TService>>? configure = null) where TService : class
		=> fixture
			.ReplaceWithMock<TService>(defaultMockBehavior, args, (_, mock) => configure?.Invoke(mock));
	
	public static DependencyContainerFixture ReplaceWithEmptyMock<TService>(
		this DependencyContainerFixture fixture,
		MockBehavior defaultMockBehavior = MockBehavior.Default,
		object[]? args = null) where TService : class
		=> fixture
			.ReplaceWithMock<TService>(defaultMockBehavior, args, (_, _) => { });
}