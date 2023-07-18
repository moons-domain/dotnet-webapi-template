using JetBrains.Annotations;
using RichWebApi.Persistence.Internal;

namespace RichWebApi.Startup;

[UsedImplicitly]
internal class AssertEntityValidatorsAction : IAsyncStartupAction
{
	private readonly IEntityValidatorsProvider _validatorsProvider;

	public uint Order => 0;

	public AssertEntityValidatorsAction(IEntityValidatorsProvider validatorsProvider)
		=> _validatorsProvider = validatorsProvider;

	public Task PerformActionAsync(CancellationToken cancellationToken = default)
	{
		if (!_validatorsProvider.AllEntitiesHaveValidators)
		{
			throw new InvalidOperationException("All entities should have their validators");
		}
		return Task.CompletedTask;
	}
}