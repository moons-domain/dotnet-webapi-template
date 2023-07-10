using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace RichWebApi.Maintenance;

internal sealed class ApplicationMaintenance : IApplicationMaintenance
{
	private readonly ILogger<ApplicationMaintenance> _logger;

	public bool IsEnabled { get; private set; }

	[MemberNotNullWhen(true, nameof(IsEnabled))] public string? Reason { get; private set; }

	public ApplicationMaintenance(ILogger<ApplicationMaintenance> logger) => _logger = logger;

	public void Enable(string reason)
	{
		if (string.IsNullOrEmpty(reason))
		{
			throw new ArgumentException("Reason should be real value", nameof(reason));
		}

		IsEnabled = true;
		Reason = reason;
		_logger.LogInformation("Maintenance mode enabled, reason: {Reason}", reason);
	}

	public void Disable()
	{
		IsEnabled = false;
		Reason = null;
		_logger.LogInformation("Maintenance mode disabled");
	}
}