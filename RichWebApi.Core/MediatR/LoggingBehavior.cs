using MediatR;
using Microsoft.Extensions.Logging;

namespace RichWebApi.MediatR;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	where TRequest : IBaseRequest
{
	private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

	public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var requestName = typeof(TRequest).Name;
		using (_logger.BeginScope(requestName))
		{
			try
			{
				return await next();
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Request execution failed");
				throw;
			}
		}
	}
}