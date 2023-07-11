using FluentValidation;
using JetBrains.Annotations;
using RichWebApi.Validation;

namespace RichWebApi.Config;

public class DatabaseConfig
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
	public class DevEnvValidator : AbstractValidator<DatabaseConfig>
	{
		public DevEnvValidator()
		{
			Include(new CommonValidator());
			RuleFor(x => x.ConnectionString).SqlServerConnectionString();
		}
	}

	private class CommonValidator : AbstractValidator<DatabaseConfig>
	{
		public CommonValidator()
		{
			RuleFor(x => x.Retries).GreaterThanOrEqualTo(0);
			RuleFor(x => x.Timeout).GreaterThan(0);
		}
	}

	[UsedImplicitly]
	public class ProdEnvValidator : AbstractValidator<DatabaseConfig>
	{
		public ProdEnvValidator()
		{
			Include(new CommonValidator());
			RuleFor(x => x.Port).GreaterThan((ushort)0);
			RuleFor(x => x.Host).MinimumLength(1);
			RuleFor(x => x.Password).MinimumLength(1);
			RuleFor(x => x.Username).MinimumLength(1);
			RuleFor(x => x.DbInstanceIdentifier).MinimumLength(1);
			RuleFor(x
					=> $"Server=tcp:{x.Host},{x.Port};Initial Catalog={x.DbInstanceIdentifier};User ID={x.Username};Password={x.Password}")
				.SqlServerConnectionString();
		}
	}
}