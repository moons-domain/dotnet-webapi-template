using System.Drawing;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RichWebApi.Extensions;
using RichWebApi.Part.Exceptions;

namespace RichWebApi.MediatR;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators;
	private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

	public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
	{
		_validators = validators;
		_logger = logger;
	}
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
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
				_logger.LogWarning("Validation failed for {Request}, failures: {Failures}",
					request,
					failures);
				throw new BadRequestException(failures.SelectMany(x => x.Errors.Select(f => f.ErrorMessage)));
			}
		}, "Request {RequestName} validation", typeof(TRequest).Name);

		return await next();
	}
}