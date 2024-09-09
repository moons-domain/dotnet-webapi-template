using MediatR;
using Microsoft.Extensions.Logging;
using RichWebApi.Extensions;

namespace RichWebApi.MediatR;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IBaseRequest
{
	public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		=> logger.TimeAsync(new Func<Task<TResponse>>(next), typeof(TRequest).Name);
}