namespace RichWebApi.Maintenance;

public class MaintenanceFailedReason : MaintenanceReason
{
	public MaintenanceInfo LastMaintenanceInfo { get; }

	public MaintenanceFailedReason(MaintenanceInfo lastMaintenanceInfo) : base($"Last maintenance operation '{lastMaintenanceInfo.Reason.Description}' failed")
		=> LastMaintenanceInfo = lastMaintenanceInfo;
}