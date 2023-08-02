using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RichWebApi.Exceptions;
using RichWebApi.Extensions;

namespace RichWebApi.MediatR;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IBaseRequest
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;
	private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

	public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators,
							  ILogger<ValidationBehavior<TRequest, TResponse>> logger)
	{
		_validators = validators;
		_logger = logger;
	}

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
										CancellationToken cancellationToken)
	{
		await _logger.TimeAsync(async () =>
		{
			var validationResults = await Task.WhenAll(_validators
				.Select(v => v.ValidateAsync(request, cancellationToken)));
			var failures = validationResults
				.Where(f => !f.IsValid)
				.ToList();

			if (failures.Any())
			{
				var errors = failures.SelectMany(x => x.Errors).ToArray();
				_logger.LogWarning("Validation failed for {Request}, failures: {@Failures}", request,
					errors.Select(e => new { e.ErrorMessage, e.PropertyName, e.ErrorCode }));
				throw new RichWebApiValidationException(errors.Select(f => f.ErrorMessage));
			}
		}, "MediatR request validation");

		return await next();
	}
}