namespace RichWebApi.Maintenance.Exceptions;

public sealed class MaintenanceAlreadyDisabledException() : MaintenanceException("Maintenance is disabled already");