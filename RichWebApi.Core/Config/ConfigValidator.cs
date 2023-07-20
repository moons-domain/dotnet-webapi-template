using FluentValidation;

namespace RichWebApi.Config;

public abstract class ConfigValidator<T> : AbstractValidator<T>, IConfigValidator where T : class, new()
{

}