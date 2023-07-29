using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RichWebApi.Entities.Configuration;

namespace RichWebApi.Entities;

public class ConfigurableEntity : IEntity
{
	public class Configurator : EntityConfiguration<ConfigurableEntity>
	{
		public override void Configure(EntityTypeBuilder<ConfigurableEntity> builder)
		{
			base.Configure(builder);
			builder.HasNoKey();
		}
	}
}