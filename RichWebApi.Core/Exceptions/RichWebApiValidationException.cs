namespace RichWebApi.Exceptions;

public class RichWebApiValidationException : RichWebApiException
{
	private const string PrimaryMessage = "The server couldn't make sense of your request";

	public RichWebApiValidationException(IEnumerable<string> errors)
		: base($"{PrimaryMessage}: {errors.Aggregate((prev, next) => $"{prev}, {next}")}")
	{
	}

	public RichWebApiValidationException(string validationError)
		: base($"{PrimaryMessage}: {validationError}")
	{
	}

	public override int StatusCode { get; } = 400;
}