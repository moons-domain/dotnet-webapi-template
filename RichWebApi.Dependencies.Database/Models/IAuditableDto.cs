namespace RichWebApi.Models;

public interface IAuditableDto
{
	DateTime CreatedAt { get; set; }
	DateTime ModifiedAt { get; set; }
}