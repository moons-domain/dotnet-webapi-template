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
		{
			RuleFor(x => x.DeletedAt)
				.GreaterThan(new DateTime(2022, 1, 1, 0, 0, 0, 0, 0, 0))
				.When(x => x.DeletedAt is not null);
		}
	}
}