using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using RichWebApi.Maintenance.Exceptions;

namespace RichWebApi.Maintenance;

public sealed class ApplicationMaintenance(ILogger<ApplicationMaintenance> logger, ISystemClock clock)
{
	public bool IsEnabled => _info != null;

	private MaintenanceInfo? _info;

	public MaintenanceInfo Info
	{
		get
		{
			if (!IsEnabled)
			{
				throw new InvalidOperationException(
					$"Maintenance is disabled{Environment.NewLine}If you want to check whether it's enabled, use '{nameof(IsEnabled)}' property");
			}

			return _info!;
		}
	}

	public void Enable(MaintenanceReason reason)
	{
		if (IsEnabled)
		{
			throw new MaintenanceAlreadyEnabledException(_info!);
		}

		_info = new MaintenanceInfo(reason)
		{
			StartedAt = clock.UtcNow.DateTime
		};
		logger.LogInformation("Maintenance mode enabled, reason: {@Reason}", reason);
	}

	public void Disable()
	{
		if (!IsEnabled)
		{
			throw new MaintenanceAlreadyDisabledException();
		}
		_info = null;
		logger.LogInformation("Maintenance mode disabled");
	}
}