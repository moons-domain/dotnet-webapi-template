using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RichWebApi.Dependencies.Database.Tests;
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

	public GetWeatherForecastTests(ITestOutputHelper testOutputHelper, ResourceRepositoryFixture resourceRepository, DependencyContainerFixture dependencyContainerFixture) : base(testOutputHelper)
	{
		_resourceScope = resourceRepository.BeginTestScope(this);
		var parts = new AppPartsCollection()
			.AddWeather();
		_serviceProvider = dependencyContainerFixture
			.WithXunitLogging(testOutputHelper)
			.WithTestScopeInMemoryDatabase(this, parts)
			.WithSystemClock()
			.ConfigureServices(s => s.ApplyParts(parts));
	}

	[Fact]
	public async Task LoadsFirstPage()
	{
		var entities = _resourceScope.GetJsonInputResource<WeatherForecast[]>("entities");
		var db = _serviceProvider.GetRequiredService<IRichWebApiDatabase>();
		foreach (var wf in entities)
		{
			await db.Context.AddAsync(wf);
		}

		await db.PersistAsync();
		
		
		var input = _resourceScope.GetJsonInputResource<GetWeatherForecast>();
		var mediator = _serviceProvider.GetRequiredService<IMediator>();
		
		var result = await mediator.Send(input);
		_resourceScope.CompareWithJsonExpectation(TestOutputHelper, result);
	}
}