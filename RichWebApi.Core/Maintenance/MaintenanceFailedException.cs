using RichWebApi.Exceptions;

namespace RichWebApi.Maintenance;

public class MaintenanceFailedException : RichWebApiException
{
	public MaintenanceInfo Info { get; }

	public MaintenanceFailedException(MaintenanceInfo info, Exception innerException) : base($"Last maintenance operation '{info.Reason.Description}' failed", innerException)
		=> Info = info;
}