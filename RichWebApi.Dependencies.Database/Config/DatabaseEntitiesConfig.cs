using FluentValidation;
using JetBrains.Annotations;

namespace RichWebApi.Config;

public class DatabaseEntitiesConfig : IAppConfig
{
	public EntitiesValidationOption Validation { get; set; }

	[UsedImplicitly]
	public class Validator : AbstractValidator<DatabaseEntitiesConfig>
	{
		public Validator()
			=> RuleFor(x => x.Validation).IsInEnum();
	}
}