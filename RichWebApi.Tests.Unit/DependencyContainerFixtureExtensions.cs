using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Internal;
using RichWebApi.Tests.DependencyInjection;

namespace RichWebApi.Tests;

public static class DependencyContainerFixtureExtensions
{
	public static DependencyContainerFixture WithSystemClock(this DependencyContainerFixture fixture)
		=> fixture.ConfigureServices(s => s.TryAddSingleton<ISystemClock, SystemClock>());
}