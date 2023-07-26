using RichWebApi.Tests.Core.Resources;
using RichWebApi.Tests.Unit;
using Xunit.Abstractions;

namespace RichWebApi.Operations;

public class GetWeatherForecastTests : UnitTest
{
	private readonly IResourceScope _resourceScope;

	public GetWeatherForecastTests(ITestOutputHelper testOutputHelper, ResourceRepositoryFixture resourceRepository) : base(testOutputHelper) 
		=> _resourceScope = resourceRepository.BeginTestScope(this);

	[Fact]
	public Task LoadsFirstPage()
	{
		var input = _resourceScope.GetJsonInputResource<GetWeatherForecast>();
		return Task.CompletedTask;
	}
}