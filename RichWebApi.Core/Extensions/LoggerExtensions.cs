using System.Diagnostics;
using Dawn;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace RichWebApi.Extensions;

public static class LoggerExtensions
{
#if DEBUG
	private const LogLevel PerformanceLoggingLevel = LogLevel.Information;
#else
private const LogLevel PerformanceLoggingLevel = LogLevel.Debug;
#endif

	private static void LogPerformanceStart(this ILogger logger, string description, params object[] args)
	{
		switch (args.Length)
		{
			case >= 1:
				logger.Log(PerformanceLoggingLevel, $"PERF: Start  [{description}]", args);
				break;
			default:
				logger.Log(
					PerformanceLoggingLevel,
					"PERF: Start  [{Description}]", description);
				break;
		}
	}
	
	private static void LogPerformanceEnd(this ILogger logger, string description, object[] args, Stopwatch sw)
	{
		switch (args.Length)
		{
			case >= 1:
				logger.Log(
					PerformanceLoggingLevel,
					$"PERF: Finish [{description}] elapsed in {{ElapsedMs}} ms",
					args.Append(sw.Elapsed.TotalMilliseconds.ToString("F")).ToArray());
				break;
			default:
				logger.Log(
					PerformanceLoggingLevel,
					$"PERF: Finish [{description}] elapsed in {{ElapsedMs}} ms",
					sw.Elapsed.TotalMilliseconds.ToString("F"));
				break;
		}
	}

	public static Task TimeAsync(this ILogger? logger,
	                             Func<Task> operation,
	                             string description,
	                             params object[] args)
	{
		if (logger is null or NullLogger || !logger.IsEnabled(PerformanceLoggingLevel))
		{
			return operation();
		}

		Guard.Argument(description, nameof(description)).NotNull().NotEmpty();
		Guard.Argument(operation, nameof(operation)).NotNull();
		logger.LogPerformanceStart(description, args);
		var sw = Stopwatch.StartNew();
		var originalTask = operation();
		originalTask.ContinueWith(t =>
		{
			sw.Stop();
			logger.LogPerformanceEnd(description, args, sw);
			if (t.Exception is not null)
			{
				logger.LogError(t.Exception, "Action [{Description}] ended with an error", description);
			}
		}, TaskContinuationOptions.ExecuteSynchronously);
		return originalTask;
	}
}