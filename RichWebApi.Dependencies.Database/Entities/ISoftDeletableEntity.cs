using FluentValidation;
using JetBrains.Annotations;

namespace RichWebApi.Entities;

public interface ISoftDeletableEntity : IEntity
{
	DateTime? DeletedAt { get; set; }

	[UsedImplicitly]
	public class Validator : AbstractValidator<ISoftDeletableEntity>
	{
		public Validator()
			=> RuleFor(x => x.DeletedAt)
				.GreaterThanOrEqualTo(EntityValidatorConstants.DefaultDateTime)
				.When(x => x.DeletedAt is not null);
	}
}