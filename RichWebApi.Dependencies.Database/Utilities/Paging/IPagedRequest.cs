using FluentValidation;

namespace RichWebApi.Utilities.Paging;

public interface IPagedRequest
{
	int Page { get; }
	int Size { get; }

	public class Validator : AbstractValidator<IPagedRequest>
	{
		public Validator()
		{
			RuleFor(x => x.Page).GreaterThanOrEqualTo(0);
			RuleFor(x => x.Size).GreaterThan(0);
		}
	}
}