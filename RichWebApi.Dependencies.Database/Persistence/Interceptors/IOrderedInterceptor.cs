using Microsoft.EntityFrameworkCore.Diagnostics;

namespace RichWebApi.Persistence.Interceptors;

public interface IOrderedInterceptor : IInterceptor
{
	uint Order { get; }
}