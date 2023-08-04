namespace RichWebApi.Maintenance;

public static class ApplicationMaintenanceExtensions
{
	public static void ExecuteInScope(this ApplicationMaintenance maintenance, Action execute, MaintenanceReason reason)
	{
		maintenance.Enable(reason);
		try
		{
			execute();
		}
		catch (Exception e)
		{
			var lastOperationInfo = maintenance.Info;
			maintenance.Enable(new MaintenanceFailedReason(lastOperationInfo));
			throw new MaintenanceFailedException(lastOperationInfo, e);
		}
		maintenance.Disable();
	}

	public static async Task ExecuteInScopeAsync(this ApplicationMaintenance maintenance, Func<Task> executeAsync,
												 MaintenanceReason reason)
	{
		maintenance.Enable(reason);
		try
		{
			await executeAsync();
		}
		catch (Exception e)
		{
			var lastOperationInfo = maintenance.Info;
			maintenance.Enable(new MaintenanceFailedReason(lastOperationInfo));
			throw new MaintenanceFailedException(lastOperationInfo, e);
		}
	}
}