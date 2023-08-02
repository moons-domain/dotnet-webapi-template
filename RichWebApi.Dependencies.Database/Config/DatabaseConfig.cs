using System.Linq.Expressions;
using FluentValidation;
using JetBrains.Annotations;
using RichWebApi.Validation;

namespace RichWebApi.Config;

internal class DatabaseConfig
{
	public string Password { get; set; } = null!;

	public ushort Port { get; set; }

	public string Host { get; set; } = null!;

	public string Username { get; set; } = null!;

	public string DbInstanceIdentifier { get; set; } = null!;

	public int Retries { get; set; }

	public int Timeout { get; set; }

	// should have value in development environment
	public string ConnectionString { get; set; } = null!;

	[UsedImplicitly]
	public class DevEnvValidator : OptionsValidator<DatabaseConfig>
	{
		public DevEnvValidator()
		{
			RuleFor(x => x.Retries).GreaterThan(0);
			RuleFor(x => x.Timeout).GreaterThan(0);
			RuleFor(x => x.ConnectionString).NotNull().NotEmpty().SqlServerConnectionString();
		}
	}

	[UsedImplicitly]
	public class ProdEnvValidator : OptionsValidator<DatabaseConfig>
	{
		public ProdEnvValidator()
		{
			void ConnectionStringPart(Expression<Func<DatabaseConfig, string>> expr)
				=> RuleFor(expr)
					.Must(x => !string.IsNullOrEmpty(x) && !x.Contains(';'))
					.WithMessage("Should be real and not contain restricted characters");

			RuleFor(x => x.Retries).GreaterThan(0);
			RuleFor(x => x.Timeout).GreaterThan(0);
			RuleFor(x => x.Port).GreaterThan((ushort)0);
			ConnectionStringPart(x => x.Host);
			ConnectionStringPart(x => x.Password);
			ConnectionStringPart(x => x.Username);
			ConnectionStringPart(x => x.DbInstanceIdentifier);
			RuleFor(x
					=> $"Server=tcp:{x.Host},{x.Port};Initial Catalog={x.DbInstanceIdentifier};User ID={x.Username};Password={x.Password}")
				.SqlServerConnectionString();
		}
	}
}