namespace RichWebApi.Maintenance.Exceptions;

public sealed class MaintenanceAlreadyDisabledException : MaintenanceException
{
	public MaintenanceAlreadyDisabledException() : base("Maintenance is disabled already")
	{
	}
}