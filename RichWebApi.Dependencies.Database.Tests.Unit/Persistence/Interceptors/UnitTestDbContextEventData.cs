using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace RichWebApi.Tests.Persistence.Interceptors;

public class UnitTestDbContextEventData : DbContextEventData
{
	public UnitTestDbContextEventData(ILoggingOptions loggingOptions, DbContext? dbContext)
		: base(new EventDefinition(loggingOptions,
				new EventId(1),
				LogLevel.Debug,
				"unit test",
				_ => (_, _) => { }),
			(_, _) => "unit test event",
			dbContext)
	{
	}
}