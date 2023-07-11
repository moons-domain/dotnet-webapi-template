using System.Data;

namespace RichWebApi.Persistence;

public interface IRichWebApiDatabase
{
	Task<T> ReadAsync<T>(Func<IRichWebApiDatabase, CancellationToken, Task<T>> func,
						 CancellationToken cancellationToken);

	Task WriteAsync(Func<IRichWebApiDatabase, CancellationToken, Task> func,
					CancellationToken cancellationToken);

	Task PersistAsync(CancellationToken cancellationToken = default);

	RichWebApiDbContext Context { get; }
}