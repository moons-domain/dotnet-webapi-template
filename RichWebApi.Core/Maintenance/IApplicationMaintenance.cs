namespace RichWebApi.Maintenance;

public interface IApplicationMaintenance
{
	bool IsEnabled { get; }
	string? Reason { get; }
	void Enable(string reason);
	void Disable();
}