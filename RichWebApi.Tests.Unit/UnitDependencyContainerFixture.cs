using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RichWebApi.Tests.DependencyInjection;

namespace RichWebApi.Tests;

[UsedImplicitly]
public class UnitDependencyContainerFixture : DependencyContainerFixture
{
	protected override IServiceCollection ConfigureSharedServices(IServiceCollection services)
		=> base.ConfigureSharedServices(services)
			.AddCore()
			.AddCoreMediatRBehaviors();
}