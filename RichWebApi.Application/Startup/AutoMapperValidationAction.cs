using AutoMapper;
using JetBrains.Annotations;

namespace RichWebApi.Startup;

[UsedImplicitly]
public class AutoMapperValidationAction : IAsyncStartupAction
{
	private readonly IMapper _mapper;

	public AutoMapperValidationAction(IMapper mapper) => _mapper = mapper;

	public uint Order => 0;

	public Task PerformActionAsync(CancellationToken cancellationToken = default)
	{
		_mapper.ConfigurationProvider.AssertConfigurationIsValid();
		_mapper.ConfigurationProvider.CompileMappings();
		return Task.CompletedTask;
	}
}