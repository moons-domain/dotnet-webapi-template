using FluentValidation;
using JetBrains.Annotations;
using MediatR;

namespace $rootnamespace$;

public record $safeitemname$ : IRequest<T>
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<$safeitemname$>
	{
		public Validator()
		{
		}
	}

	[UsedImplicitly]
	internal class $safeitemname$Handler : IRequestHandler<$safeitemname$, T>
	{
		public $safeitemname$Handler()
		{
		}

		public Task<T> Handle($safeitemname$ request, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}