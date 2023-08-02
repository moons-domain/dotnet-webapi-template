namespace RichWebApi.Models;

public interface ISoftDeletableDto
{
	DateTime? DeletedAt { get; set; }
}