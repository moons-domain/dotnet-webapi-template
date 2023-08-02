using JetBrains.Annotations;
using RichWebApi.Entities.Configuration;

namespace RichWebApi.Entities;

[UsedImplicitly]
public class IgnoredEntity : IEntity
{
	[UsedImplicitly]
	public class Configurator : EntityConfiguration<IgnoredEntity>, IIgnoredEntityConfiguration
	{
	}
}