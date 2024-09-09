using FluentValidation;
using FluentValidation.Results;
using Microsoft.Data.SqlClient;

namespace RichWebApi.Validation;

internal static class RuleBuilderExtensions
{
	public static IRuleBuilder<T, string> SqlServerConnectionString<T>(this IRuleBuilder<T, string> builder) => builder
		.Custom((value, context) =>
		{
			try
			{
				var _ = new SqlConnectionStringBuilder(value);
			}
			catch
			{
				context.AddFailure(
					new ValidationFailure(
						context.PropertyPath,
						$"'{context.PropertyPath}' does not contain a valid SQL Server connection string.",
						value));
			}
		});
}