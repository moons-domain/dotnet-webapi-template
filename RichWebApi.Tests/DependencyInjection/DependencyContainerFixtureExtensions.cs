﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RichWebApi.Tests.Logging;
using Xunit.Abstractions;

namespace RichWebApi.Tests.DependencyInjection;

public static class DependencyContainerFixtureExtensions
{
	public static DependencyContainerFixture WithXunitLogging(this DependencyContainerFixture container,
	                                                          ITestOutputHelper testOutputHelper)
		=> container.ConfigureServices(services => services.AddLogging(x =>
		{
			x.ClearProviders();
			x.SetMinimumLevel(LogLevel.Information);
			x.AddProvider(new XunitLoggerProvider(LogLevel.Trace, testOutputHelper));
		}));
}