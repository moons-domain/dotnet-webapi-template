using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

namespace RichWebApi.Maintenance;

public sealed class ApplicationMaintenance
{
	private readonly ILogger<ApplicationMaintenance> _logger;
	private readonly ISystemClock _clock;

	public bool IsEnabled => _info != null;

	private MaintenanceInfo? _info;

	public MaintenanceInfo Info
	{
		get
		{
			if (!IsEnabled)
			{
				throw new InvalidOperationException(
					$"Maintenance is disabled{Environment.NewLine}If you want to check whether it's enabled, use IsEnabled property");
			}

			return _info!;
		}
	}

	public ApplicationMaintenance(ILogger<ApplicationMaintenance> logger, ISystemClock clock)
	{
		_logger = logger;
		_clock = clock;
	}

	public void Enable(MaintenanceReason reason)
	{
		_info = new MaintenanceInfo(reason)
		{
			StartedAt = _clock.UtcNow.DateTime
		};
		_logger.LogInformation("Maintenance mode enabled, reason: {@Reason}", reason);
	}

	public void Disable()
	{
		_info = null;
		_logger.LogInformation("Maintenance mode disabled");
	}
}