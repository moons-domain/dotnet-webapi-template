using RichWebApi.Exceptions;

namespace RichWebApi.Maintenance.Exceptions;

public abstract class MaintenanceException : RichWebApiException
{
	protected MaintenanceException(string message) : base(message)
	{
	}

	protected MaintenanceException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}