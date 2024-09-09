using RichWebApi.Utilities;

namespace RichWebApi.Persistence;

internal sealed class RichWebApiDatabase(RichWebApiDbContext context, IDatabasePolicySet policySet)
	: IRichWebApiDatabase
{
	public Task<T> ReadAsync<T>(Func<IRichWebApiDatabase, CancellationToken, Task<T>> func,
								CancellationToken cancellationToken)
		=> policySet.DatabaseReadPolicy.ExecuteAsync(ct => func(this, ct), cancellationToken);

	public Task WriteAsync(Func<IRichWebApiDatabase, CancellationToken, Task> func, CancellationToken cancellationToken)
		=> policySet.DatabaseWritePolicy.ExecuteAsync(ct => func(this, ct), cancellationToken);

	public Task PersistAsync(CancellationToken cancellationToken = default)
		=> WriteAsync((db, ct) => db.Context.SaveChangesAsync(ct), cancellationToken);

	public RichWebApiDbContext Context { get; } = context;
}