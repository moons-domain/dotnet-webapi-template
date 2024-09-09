namespace RichWebApi.Maintenance.Exceptions;

public class MaintenanceScopeFailedException(MaintenanceInfo info, Exception innerException)
	: MaintenanceException($"Last maintenance operation '{info.Reason.Description}' failed", innerException)
{
	public MaintenanceInfo Info { get; } = info;
}