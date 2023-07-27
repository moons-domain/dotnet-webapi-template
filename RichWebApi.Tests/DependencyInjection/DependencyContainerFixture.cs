using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace RichWebApi.Tests.DependencyInjection;

[UsedImplicitly]
public class DependencyContainerFixture : IDisposable
{
	private readonly IServiceCollection _services = new ServiceCollection();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DependencyContainerFixture ConfigureServices(Action<IServiceCollection> configure)
	{
		configure.Invoke(_services);
		return this;
	}

	public IServiceProvider BuildServiceProvider()
		=> _services.BuildServiceProvider();
	
	public void Dispose() => _services.Clear();
}