using FluentValidation;

namespace RichWebApi.Config;

public abstract class OptionsValidator<T> : AbstractValidator<T>, IOptionsValidator where T : class, new()
{

}