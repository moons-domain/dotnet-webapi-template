using RichWebApi.Utilities;

namespace RichWebApi.Persistence;

internal sealed class RichWebApiDatabase : IRichWebApiDatabase
{
	private readonly IDatabasePolicySet _policySet;

	public RichWebApiDatabase(RichWebApiDbContext context, IDatabasePolicySet policySet)
	{
		_policySet = policySet;
		Context = context;
	}

	public Task<T> ReadAsync<T>(Func<IRichWebApiDatabase, CancellationToken, Task<T>> func,
								CancellationToken cancellationToken)
		=> _policySet.DatabaseReadPolicy.ExecuteAsync(ct => func(this, ct), cancellationToken);

	public Task WriteAsync(Func<IRichWebApiDatabase, CancellationToken, Task> func, CancellationToken cancellationToken)
		=> _policySet.DatabaseWritePolicy.ExecuteAsync(ct => func(this, ct), cancellationToken);

	public Task PersistAsync(CancellationToken cancellationToken = default)
		=> WriteAsync((db, ct) => db.Context.SaveChangesAsync(ct), cancellationToken);

	public RichWebApiDbContext Context { get; }
}