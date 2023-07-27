using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RichWebApi.Entities;
using RichWebApi.Persistence;
using RichWebApi.Tests;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Resources;
using Xunit.Abstractions;

namespace RichWebApi.Operations;

public class GetWeatherForecastTests : UnitTest
{
	private readonly IResourceScope _resourceScope;
	private readonly IServiceProvider _serviceProvider;

	public GetWeatherForecastTests(ITestOutputHelper testOutputHelper,
	                               ResourceRepositoryFixture resourceRepository,
	                               DependencyContainerFixture container) : base(testOutputHelper)
	{
		_resourceScope = resourceRepository.BeginTestScope(this);
		var parts = new AppPartsCollection()
			.AddWeather();
		_serviceProvider = container
			.WithXunitLogging(TestOutputHelper)
			.WithTestScopeInMemoryDatabase(parts)
			.WithSystemClock()
			.ConfigureServices(s => s.AddAppParts(parts))
			.BuildServiceProvider();
	}

	[Fact]
	public async Task LoadsFirstPage()
	{
		var entities = _resourceScope.GetJsonInputResource<WeatherForecast[]>("entities");
		await PersistEntitiesAsync(entities);

		var input = _resourceScope.GetJsonInputResource<GetWeatherForecast>();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		var result = await mediator.Send(input);
		_resourceScope.CompareWithJsonExpectation(TestOutputHelper, result);
	}

	[Fact]
	public async Task LoadsOne()
	{
		var entities = _resourceScope.GetJsonInputResource<WeatherForecast[]>("entities");
		await PersistEntitiesAsync(entities);

		var input = _resourceScope.GetJsonInputResource<GetWeatherForecast>();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();

		var result = await mediator.Send(input);
		_resourceScope.CompareWithJsonExpectation(TestOutputHelper, result);
	}

	private async ValueTask PersistEntitiesAsync<T>(IEnumerable<T> entities) where T : class, IEntity
	{
		var db = _serviceProvider.GetRequiredService<IRichWebApiDatabase>();

		foreach (var e in entities)
		{
			await db.Context.AddAsync(e);
		}

		await db.PersistAsync();
	}
}