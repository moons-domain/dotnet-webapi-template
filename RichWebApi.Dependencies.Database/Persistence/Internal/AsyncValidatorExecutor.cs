using FluentValidation.Results;

namespace RichWebApi.Persistence.Internal;

internal delegate Task<ValidationResult> AsyncValidationExecutor(object validator,
                                                                 object entity,
                                                                 CancellationToken cancellationToken);