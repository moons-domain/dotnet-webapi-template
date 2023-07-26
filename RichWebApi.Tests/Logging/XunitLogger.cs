using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace RichWebApi.Tests.Logging;

public sealed class XunitLogger : ILogger
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _name;
    private readonly LogLevel _loggerLevel;

    public XunitLogger(string name, LogLevel loggerLevel, ITestOutputHelper testOutputHelper)
    {
        _name = name;
        _loggerLevel = loggerLevel;
        _testOutputHelper = testOutputHelper;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _loggerLevel;

    public void Log<TState>(LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (IsEnabled(logLevel))
        {
            _testOutputHelper.WriteLine($"[{DateTime.UtcNow:HH:mm:ss.fff}] " +
                                       $"[{logLevel.ToString("G")[..4].ToUpper()}] " +
                                       $"[{_name}] " +
                                       $"{formatter(state, exception)}");
        }
    }
}