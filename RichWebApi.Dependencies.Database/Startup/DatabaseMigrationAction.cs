using JetBrains.Annotations;
using RichWebApi.Utilities;

namespace RichWebApi.Startup;

[UsedImplicitly]
public class DatabaseMigrationAction(RichWebApiDbContext context) : IAsyncStartupAction
{
	public uint Order => 1;

	public async Task PerformActionAsync(CancellationToken cancellationToken = default)
	{
		using (new NoTimeoutDbContextScope(context))
		{
			await context.Database.MigrateAsync(cancellationToken);
		}
	}
}