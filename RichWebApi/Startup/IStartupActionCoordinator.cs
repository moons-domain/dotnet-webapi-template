namespace RichWebApi.Startup;

public interface IStartupActionCoordinator
{
	Task PerformStartupActionsAsync(CancellationToken cancellationToken);
}