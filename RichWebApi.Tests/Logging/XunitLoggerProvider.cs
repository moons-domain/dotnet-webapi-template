using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace RichWebApi.Tests.Logging;

[ProviderAlias("Xunit")]
public sealed class XunitLoggerProvider : ILoggerProvider
{
    private readonly LogLevel _level;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ConcurrentDictionary<string, XunitLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

    public XunitLoggerProvider(LogLevel level, ITestOutputHelper testOutputHelper)
    {
        _level = level;
        _testOutputHelper = testOutputHelper;
    }

    public ILogger CreateLogger(string categoryName)
        => _loggers.GetOrAdd(categoryName,
            static (name, args) => new XunitLogger(name, args.level, args.testOutputHelper),
            (level: _level, testOutputHelper: _testOutputHelper));

    public void Dispose() => _loggers.Clear();
}