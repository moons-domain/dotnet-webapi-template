using RichWebApi.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace RichWebApi.MediatR;

public sealed class PerformanceLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	private readonly ILogger<PerformanceLoggingBehavior<TRequest, TResponse>> _logger;

	public PerformanceLoggingBehavior(ILogger<PerformanceLoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

	public Task<TResponse> Handle(TRequest request,
								  RequestHandlerDelegate<TResponse> next,
								  CancellationToken cancellationToken)
		=> _logger.TimeAsync(new Func<Task<TResponse>>(next), "Request {RequestName} performance", typeof(TRequest).Name);
}