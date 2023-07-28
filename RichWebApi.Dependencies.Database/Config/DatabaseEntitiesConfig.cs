using FluentValidation;
using JetBrains.Annotations;

namespace RichWebApi.Config;

public class DatabaseEntitiesConfig
{
	public EntitiesValidationOption Validation { get; set; }

	[UsedImplicitly]
	public class Validator : OptionsValidator<DatabaseEntitiesConfig>
	{
		public Validator() 
			=> RuleFor(x => x.Validation).IsInEnum();
	}
}