using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RichWebApi.Tests.DependencyInjection;
using Serilog;
using Serilog.Extensions.Logging;
using Xunit.Abstractions;

namespace RichWebApi.Tests.Logging;

public static class DependencyContainerFixtureExtensions
{
	public static DependencyContainerFixture WithXunitLogging(this DependencyContainerFixture container,
	                                                          ITestOutputHelper testOutputHelper)
	{
		var serilogLogger = new LoggerConfiguration()
			.MinimumLevel.Verbose()
			.Enrich.FromLogContext()
			.WriteTo.TestOutput(testOutputHelper, outputTemplate: "[{Timestamp:HH:mm:ss.fff}] "
			                                                      + "[{RequestId}] "
			                                                      + "[{SourceContext:l}] "
			                                                      + "[{Level:u3}] "
			                                                      + "{Message:lj}{NewLine}"
			                                                      + "{Properties:j}{NewLine}"
			                                                      + "{Exception}")
			.CreateLogger();
		return container.ConfigureServices(services => services.AddLogging(x =>
		{
			x.ClearProviders();
			x.SetMinimumLevel(LogLevel.Information);
			x.AddProvider(new SerilogLoggerProvider(serilogLogger));
		}));
	}
}