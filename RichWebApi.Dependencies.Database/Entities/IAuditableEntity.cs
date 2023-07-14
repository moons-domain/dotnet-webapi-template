
namespace RichWebApi.Entities;

public interface IAuditableEntity : IEntity
{
	public DateTime CreatedAt { get; set; }

	public DateTime ModifiedAt { get; set; }
}