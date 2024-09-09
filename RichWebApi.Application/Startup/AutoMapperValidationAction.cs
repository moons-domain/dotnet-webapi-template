using AutoMapper;
using JetBrains.Annotations;

namespace RichWebApi.Startup;

[UsedImplicitly]
public class AutoMapperValidationAction(IMapper mapper) : IAsyncStartupAction
{
	public uint Order => 0;

	public Task PerformActionAsync(CancellationToken cancellationToken = default)
	{
		mapper.ConfigurationProvider.AssertConfigurationIsValid();
		mapper.ConfigurationProvider.CompileMappings();
		return Task.CompletedTask;
	}
}