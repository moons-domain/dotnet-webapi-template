using FluentValidation;
using JetBrains.Annotations;
using MediatR;

namespace $rootnamespace$;

public record $safeitemname$ : IRequest
{
	[UsedImplicitly]
    public class Validator : AbstractValidator<$safeitemname$>
    {
        public Validator()
        {
        }
    }

    [UsedImplicitly]
    internal class $safeitemname$Handler : IRequestHandler<$safeitemname$>
	{
		public $safeitemname$Handler()
        {
        }

        public Task Handle($safeitemname$ request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}