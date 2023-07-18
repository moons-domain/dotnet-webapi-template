using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace RichWebApi.Entities.Configuration;

public interface IDatabaseConfigurator
{
	void OnModelCreating(ModelBuilder modelBuilder);
	void ConfigureConventions(ModelConfigurationBuilder modelConfigurationBuilder, DatabaseFacade databaseFacade);
}