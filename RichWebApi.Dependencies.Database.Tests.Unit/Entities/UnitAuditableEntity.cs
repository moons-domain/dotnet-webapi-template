using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RichWebApi.Entities;
using RichWebApi.Entities.Configuration;

namespace RichWebApi.Tests.Entities;

public class UnitAuditableEntity : IAuditableEntity, ISoftDeletableEntity
{
	public long Id { get; set; }
	public DateTime CreatedAt { get; set; }

	public DateTime ModifiedAt { get; set; }

	public DateTime? DeletedAt { get; set; }

	public bool Invalid { get; set; }

	public class Configurator : EntityConfiguration<UnitAuditableEntity>
	{
		public override void Configure(EntityTypeBuilder<UnitAuditableEntity> builder)
		{
			base.Configure(builder);
			builder.HasKey(x => x.Id);
		}
	}

	[UsedImplicitly]
	public class Validator : AbstractValidator<UnitAuditableEntity>
	{
		public Validator()
		{
			RuleFor(x => x.Invalid).Equal(false);
		}
	}
}