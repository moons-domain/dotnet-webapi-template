using RichWebApi.Entities.Configuration;

namespace RichWebApi.Entities;

public class ConfigurableEntity : IEntity
{
	public class Configurator : EntityConfiguration<ConfigurableEntity>
	{
	}
}