using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace RichWebApi.Tests.Core.Logging;

[ProviderAlias("XUnit")]
public sealed class XUnitLoggerProvider : ILoggerProvider
{
    private readonly LogLevel _level;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ConcurrentDictionary<string, XUnitLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

    public XUnitLoggerProvider(LogLevel level, ITestOutputHelper testOutputHelper)
    {
        _level = level;
        _testOutputHelper = testOutputHelper;
    }

    public ILogger CreateLogger(string categoryName)
        => _loggers.GetOrAdd(categoryName,
            static (name, args) => new XUnitLogger(name, args.level, args.testOutputHelper),
            (level: _level, testOutputHelper: _testOutputHelper));

    public void Dispose() => _loggers.Clear();
}