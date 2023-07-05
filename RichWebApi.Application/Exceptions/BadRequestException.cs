namespace RichWebApi.Exceptions;

public class BadRequestException : RichWebApiException
{
	private const string PrimaryMessage = "The server couldn't make sense of your request";

	public BadRequestException(IEnumerable<string> errors)
		: base($"{PrimaryMessage}: {errors.Aggregate((prev, next) => $"{prev}, {next}")}")
	{
	}

	public BadRequestException(string validationError)
		: base($"{PrimaryMessage}: {validationError}")
	{
	}

	public override int StatusCode { get; } = 400;
}