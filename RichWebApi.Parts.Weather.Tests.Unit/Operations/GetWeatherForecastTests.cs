using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RichWebApi.Entities;
using RichWebApi.Persistence;
using RichWebApi.Tests;
using RichWebApi.Tests.Logging;
using RichWebApi.Tests.Resources;
using Xunit.Abstractions;

namespace RichWebApi.Operations;

public class GetWeatherForecastTests : UnitTest
{
	private readonly IResourceScope _testResources;
	private readonly IServiceProvider _serviceProvider;

	public GetWeatherForecastTests(ITestOutputHelper testOutputHelper,
	                               ResourceRepositoryFixture resourceRepository,
	                               UnitDependencyContainerFixture container) : base(testOutputHelper)
	{
		_testResources = resourceRepository.CreateTestScope(this);
		var parts = new AppPartsCollection()
			.AddWeather();
		_serviceProvider = container
			.WithXunitLogging(TestOutputHelper)
			.WithTestScopeInMemoryDatabase(parts)
			.ConfigureServices(s => s.AddAppParts(parts))
			.BuildServiceProvider();
	}

	[Fact]
	public async Task LoadsFirstPage()
	{
		using var resources = _testResources.CreateMethodScope(); 
		var entities = resources.GetJsonInputResource<WeatherForecast[]>("entities");
		await _serviceProvider
			.GetRequiredService<IRichWebApiDatabase>()
			.PersistEntitiesAsync(entities);

		var input = resources.GetJsonInputResource<GetWeatherForecast>();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		var result = await mediator.Send(input);
		resources.CompareWithJsonExpectation(TestOutputHelper, result);
	}

	[Fact]
	public async Task LoadsOne()
	{
		using var resources = _testResources.CreateMethodScope(); 
		var entities = resources.GetJsonInputResource<WeatherForecast[]>("entities");
		await _serviceProvider
			.GetRequiredService<IRichWebApiDatabase>()
			.PersistEntitiesAsync(entities);

		var input = resources.GetJsonInputResource<GetWeatherForecast>();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		var result = await mediator.Send(input);
		resources.CompareWithJsonExpectation(TestOutputHelper, result);
	}
}