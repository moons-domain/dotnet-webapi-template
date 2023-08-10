namespace RichWebApi.Maintenance;

internal sealed class MaintenanceScopeFailedReason : MaintenanceReason
{
	public MaintenanceInfo Info { get; }

	public MaintenanceScopeFailedReason(MaintenanceInfo info) : base($"Last maintenance operation '{info.Reason.Description}' failed")
		=> Info = info;
}