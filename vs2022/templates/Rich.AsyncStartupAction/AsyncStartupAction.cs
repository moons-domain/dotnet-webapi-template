using JetBrains.Annotations;

namespace $rootnamespace$;

[UsedImplicitly]
internal sealed class $safeitemname$ : IAsyncStartupAction
{
	public uint Order => 0;

	public $safeitemname$()
	{
	}

	public Task PerformActionAsync(CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}