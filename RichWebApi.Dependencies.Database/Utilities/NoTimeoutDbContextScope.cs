namespace RichWebApi.Utilities;

internal sealed class NoTimeoutDbContextScope : IDisposable
{
	private readonly DbContext _dbContext;
	private readonly int? _oldTimeout;

	public NoTimeoutDbContextScope(DbContext dbContext)
	{
		_dbContext = dbContext;
		_oldTimeout = _dbContext.Database.GetCommandTimeout();
		dbContext.Database.SetCommandTimeout(ushort.MaxValue);
	}

	public void Dispose() => _dbContext.Database.SetCommandTimeout(_oldTimeout);
}