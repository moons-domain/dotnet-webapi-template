using RichWebApi.Exceptions;

namespace RichWebApi.Maintenance.Exceptions;

public class MaintenanceScopeFailedException : MaintenanceException
{
	public MaintenanceInfo Info { get; }

	public MaintenanceScopeFailedException(MaintenanceInfo info, Exception innerException) : base($"Last maintenance operation '{info.Reason.Description}' failed", innerException)
		=> Info = info;
}