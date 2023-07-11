namespace RichWebApi.Entities;

public interface ISoftDeletableEntity : IEntity
{
	DateTime? DeletedAt { get; set; }
}