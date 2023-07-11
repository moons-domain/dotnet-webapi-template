namespace RichWebApi.Persistence;

public interface ISaveChangesReactor
{
	uint Order { get; }
	ValueTask ReactAsync(RichWebApiDbContext context, CancellationToken cancellationToken = default);
}