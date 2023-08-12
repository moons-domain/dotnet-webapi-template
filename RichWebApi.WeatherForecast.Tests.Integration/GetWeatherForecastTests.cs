using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RichWebApi.Tests.Client;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Logging;
using Xunit.Abstractions;

namespace RichWebApi.Tests;

public class GetWeatherForecastTests : IntegrationTest
{
	private readonly IServiceProvider _serviceProvider;

	public GetWeatherForecastTests(ITestOutputHelper testOutputHelper, IntegrationDependencyContainerFixture container)
		: base(testOutputHelper)
		=> _serviceProvider = container
			.WithXunitLogging(TestOutputHelper)
			.BuildServiceProvider();

	[Fact]
	public async Task ReturnsTomorrowForecast()
	{
		var tomorrow = DateTimeOffset.Now.AddDays(1);
		var client = _serviceProvider.GetRequiredService<IWeatherForecastClient>();
		var response = await client.GetWeatherForecastAsync(tomorrow);
		response.StatusCode.Should().Be(200);
		var forecast = response.Result;
		forecast.Date.Should().Be(tomorrow.Date);
	}
}