namespace RichWebApi.Maintenance;

internal sealed class MaintenanceScopeFailedReason(MaintenanceInfo info)
	: MaintenanceReason($"Last maintenance operation '{info.Reason.Description}' failed")
{
	public MaintenanceInfo Info { get; } = info;
}