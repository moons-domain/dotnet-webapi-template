namespace RichWebApi.Startup;

public interface IAsyncStartupAction
{
	uint Order { get; }

	Task PerformActionAsync(CancellationToken cancellationToken = default);
}