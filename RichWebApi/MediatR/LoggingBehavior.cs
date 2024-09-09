using MediatR;
using Microsoft.Extensions.Logging;
using RichWebApi.Extensions;

namespace RichWebApi.MediatR;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IBaseRequest
{
	private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

	public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

	public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		=> _logger.TimeAsync(new Func<Task<TResponse>>(next), typeof(TRequest).Name);
}