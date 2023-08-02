using JetBrains.Annotations;
using RichWebApi.Utilities;

namespace RichWebApi.Startup;

[UsedImplicitly]
public class DatabaseMigrationAction : IAsyncStartupAction
{
	private readonly RichWebApiDbContext _context;

	public uint Order => 1;

	public DatabaseMigrationAction(RichWebApiDbContext context) => _context = context;

	public async Task PerformActionAsync(CancellationToken cancellationToken = default)
	{
		using (new NoTimeoutDbContextScope(_context))
		{
			await _context.Database.MigrateAsync(cancellationToken);
		}
	}
}