using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace RichWebApi.Tests.DependencyInjection;

[UsedImplicitly]
public class UnitDependencyContainerFixture : DependencyContainerFixture
{
	protected override IServiceCollection ConfigureSharedServices(IServiceCollection services)
		=> base.ConfigureSharedServices(services)
			.AddCore()
			.AddCoreMediatRBehaviors();
}