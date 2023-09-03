namespace RichWebApi.Maintenance;

public class MaintenanceReason
{
	public string Description { get; }

	public MaintenanceReason(string description)
	{
		if (string.IsNullOrEmpty(description))
		{
			throw new ArgumentException("Maintenance description should be real", nameof(description));
		}

		Description = description;
	}
}