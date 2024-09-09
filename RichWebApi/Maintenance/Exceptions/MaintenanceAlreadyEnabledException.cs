namespace RichWebApi.Maintenance.Exceptions;

public sealed class MaintenanceAlreadyEnabledException(MaintenanceInfo info)
	: MaintenanceException("Maintenance is enabled already")
{
	public MaintenanceInfo Info { get; } = info;
}