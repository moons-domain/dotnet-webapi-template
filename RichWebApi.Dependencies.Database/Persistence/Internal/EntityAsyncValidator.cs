using FluentValidation.Results;

namespace RichWebApi.Persistence.Internal;

internal delegate Task<ValidationResult> EntityAsyncValidator(object entity, CancellationToken cancellationToken);