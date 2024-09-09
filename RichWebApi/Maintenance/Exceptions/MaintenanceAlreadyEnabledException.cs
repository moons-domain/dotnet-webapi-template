namespace RichWebApi.Maintenance.Exceptions;

public sealed class MaintenanceAlreadyEnabledException : MaintenanceException
{
	public MaintenanceInfo Info { get; }

	public MaintenanceAlreadyEnabledException(MaintenanceInfo info) : base("Maintenance is enabled already") => Info = info;
}