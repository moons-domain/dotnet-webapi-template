using Microsoft.EntityFrameworkCore;

namespace RichWebApi.Entities.Configuration;

public interface INonGenericEntityConfiguration
{

}

public interface IEntityConfiguration<T> : IEntityTypeConfiguration<T>, INonGenericEntityConfiguration where T : class, IEntity
{

}