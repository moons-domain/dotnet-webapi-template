using JetBrains.Annotations;
using RichWebApi.Entities;
using RichWebApi.Entities.Configuration;

namespace RichWebApi.Tests.Entities;

[UsedImplicitly]
public class IgnoredEntity : IEntity
{
	[UsedImplicitly]
	public class Configurator : EntityConfiguration<IgnoredEntity>, IIgnoredEntityConfiguration
	{
	}
}