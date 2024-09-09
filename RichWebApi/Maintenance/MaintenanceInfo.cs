namespace RichWebApi.Maintenance;

public class MaintenanceInfo(MaintenanceReason reason)
{
	public MaintenanceReason Reason { get; } = reason;

	public DateTime StartedAt { get; set; }
}