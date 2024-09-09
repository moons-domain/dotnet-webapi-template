using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RichWebApi.Tests.Client;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Logging;
using Xunit.Abstractions;

namespace RichWebApi.Tests;

public class GetWeatherForecastsTests : IntegrationTest
{
	private readonly IServiceProvider _serviceProvider;

	public GetWeatherForecastsTests(ITestOutputHelper testOutputHelper, IntegrationDependencyContainerFixture container)
		: base(testOutputHelper)
		=> _serviceProvider = container
			.WithXunitLogging(TestOutputHelper)
			.BuildServiceProvider();

	[Fact]
	public async Task ReturnsCurrentWeekForecasts()
	{
		var now = DateTimeOffset.UtcNow;
		var today = new DateTimeOffset(DateOnly.FromDateTime(now.UtcDateTime), TimeOnly.MinValue, TimeSpan.Zero);
		var client = _serviceProvider.GetRequiredService<IWeatherForecastClient>();
		var pageResponse = await client.GetWeatherForecastsAsync(0, 100, today, today.AddDays(7));
		pageResponse.StatusCode
			.Should()
			.Be(200);
		var page = pageResponse.Result;
		page.Total.Should().Be(7);
		page.Page.Should().Be(0);
		page.Size.Should().Be(100);
		var items = page.Items!;
		items.Count.Should().Be(7);
		items.DistinctBy(x => x.Date)
			.Count()
			.Should()
			.Be(7);
	}
}