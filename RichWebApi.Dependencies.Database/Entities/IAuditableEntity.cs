
using FluentValidation;
using JetBrains.Annotations;

namespace RichWebApi.Entities;

public interface IAuditableEntity : IEntity
{
	public DateTime CreatedAt { get; set; }

	public DateTime ModifiedAt { get; set; }

	[UsedImplicitly]
	public class Validator : AbstractValidator<IAuditableEntity>
	{
		public Validator()
		{
			var defaultDateTime = new DateTime(2022, 1, 1, 0, 0, 0, 0, 0);
			RuleFor(x => x.CreatedAt).GreaterThan(defaultDateTime);
			RuleFor(x => x.ModifiedAt).GreaterThan(defaultDateTime);
		}
	}
}