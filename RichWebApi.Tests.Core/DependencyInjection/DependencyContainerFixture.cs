using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RichWebApi.Tests.Core.Exceptions;

namespace RichWebApi.Tests.Core.DependencyInjection;

public class DependencyContainerFixture : IServiceProvider, IDisposable
{
	private readonly IServiceCollection _services = new ServiceCollection();
	private readonly Lazy<IServiceProvider> _lazyServiceProvider;
	private IServiceProvider ServiceProvider => _lazyServiceProvider.Value;

	public DependencyContainerFixture()
		=> _lazyServiceProvider = new Lazy<IServiceProvider>(() =>
		{
			var sp = _services.BuildServiceProvider();
			var logger = sp.GetRequiredService<ILogger<DependencyContainerFixture>>();
			logger.LogInformation("Service provider initialized");
			return sp;
		});

	public DependencyContainerFixture ConfigureServices(Action<IServiceCollection> configure)
	{
		if (_lazyServiceProvider.IsValueCreated)
		{
			throw new TestConfigurationException(
				"Services should be configured before service provider is initialized");
		}
		configure.Invoke(_services);
		return this;
	}

	public object? GetService(Type serviceType)
		=> ServiceProvider.GetService(serviceType);

	public void Dispose()
	{
	}
}