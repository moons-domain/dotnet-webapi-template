namespace RichWebApi.Maintenance;

public class MaintenanceInfo
{
	public MaintenanceReason Reason { get; }

	public MaintenanceInfo(MaintenanceReason reason) => Reason = reason;

	public DateTime StartedAt { get; set; }
}