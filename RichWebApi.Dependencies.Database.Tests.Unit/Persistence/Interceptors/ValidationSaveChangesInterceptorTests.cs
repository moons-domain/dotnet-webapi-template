using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RichWebApi.Config;
using RichWebApi.Entities;
using RichWebApi.Tests;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Logging;
using RichWebApi.Tests.Moq;
using Xunit.Abstractions;

namespace RichWebApi.Persistence.Interceptors;

public class ValidationSaveChangesInterceptorTests : UnitTest
{
	private readonly DependencyContainerFixture _container;

	public ValidationSaveChangesInterceptorTests(ITestOutputHelper testOutputHelper,
												 UnitDependencyContainerFixture container) : base(testOutputHelper)
	{
		var parts = new AppPartsCollection
		{
			new DatabaseUnitTestsPart()
		};
		_container = container
			.WithXunitLogging(testOutputHelper)
			.ReplaceWithEmptyMock<ILoggingOptions>()
			.WithTestScopeInMemoryDatabase(parts)
			.ReplaceWithEmptyMock<IValidator<ConfigurableEntity>>()
			.ReplaceWithEmptyMock<IValidator<IgnoredEntity>>()
			.ConfigureServices(s => s.AddAppParts(parts));
	}

	[Fact]
	public async Task DontDoAnythingIfUnknownDbContext()
	{
		var sp = _container
			.SetDatabaseEntitiesConfig(EntitiesValidationOption.Required)
			.BuildServiceProvider();
		var eventData = new UnitTestDbContextEventData(sp.GetRequiredService<ILoggingOptions>(), null);
		var interceptor = new ValidationSaveChangesInterceptor(sp,
			sp.GetRequiredService<ILogger<ValidationSaveChangesInterceptor>>(),
			sp.GetRequiredService<IOptionsMonitor<DatabaseEntitiesConfig>>());

		await interceptor.SavingChangesAsync(eventData, default);

		var optionsMock = sp.GetRequiredService<Mock<IOptionsMonitor<DatabaseEntitiesConfig>>>();
		optionsMock.Verify(x => x.CurrentValue, Times.Never());
	}

	[Fact]
	public async Task DontDoAnythingIfValidationNone()
	{
		var sp = _container
			.SetDatabaseEntitiesConfig(EntitiesValidationOption.None)
			.BuildServiceProvider();
		var dbContext = sp.GetRequiredService<RichWebApiDbContext>();
		await dbContext.AddAsync(new UnitAuditableEntity
		{
			Invalid = true
		});
		var eventData = new UnitTestDbContextEventData(sp.GetRequiredService<ILoggingOptions>(), dbContext);
		var interceptor = new ValidationSaveChangesInterceptor(sp,
			sp.GetRequiredService<ILogger<ValidationSaveChangesInterceptor>>(),
			sp.GetRequiredService<IOptionsMonitor<DatabaseEntitiesConfig>>());

		await interceptor.SavingChangesAsync(eventData, default);

		var optionsMock = sp.GetRequiredService<Mock<IOptionsMonitor<DatabaseEntitiesConfig>>>();
		optionsMock.Verify(x => x.CurrentValue, Times.Once());
	}

	[Fact]
	public async Task CallsProvidedValidator()
	{
		var entity = new UnitAuditableEntity();
		var sp = _container
			.SetDatabaseEntitiesConfig(EntitiesValidationOption.Required)
			.ReplaceWithMock<IValidator<UnitAuditableEntity>>(mock => mock
				.Setup(x => x.ValidateAsync(It.Is<UnitAuditableEntity>(e => e == entity),
					It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResult()))
			.BuildServiceProvider();
		var dbContext = sp.GetRequiredService<RichWebApiDbContext>();
		await dbContext.AddAsync(entity);
		var eventData = new UnitTestDbContextEventData(sp.GetRequiredService<ILoggingOptions>(), dbContext);
		var interceptor = new ValidationSaveChangesInterceptor(sp,
			sp.GetRequiredService<ILogger<ValidationSaveChangesInterceptor>>(),
			sp.GetRequiredService<IOptionsMonitor<DatabaseEntitiesConfig>>());

		await interceptor.SavingChangesAsync(eventData, default);

		var validatorMock = sp.GetRequiredService<Mock<IValidator<UnitAuditableEntity>>>();
		validatorMock.Verify(
			x => x.ValidateAsync(It.Is<UnitAuditableEntity>(e => e == entity), It.IsAny<CancellationToken>()),
			Times.Once());
	}

	[Fact]
	public async Task ThrowsIfSomeOfEntitiesAreInvalid()
	{
		var entity = new UnitAuditableEntity
		{
			Invalid = true
		};
		var sp = _container
			.SetDatabaseEntitiesConfig(EntitiesValidationOption.Required)
			.BuildServiceProvider();
		var dbContext = sp.GetRequiredService<RichWebApiDbContext>();
		await dbContext.AddAsync(entity);
		var eventData = new UnitTestDbContextEventData(sp.GetRequiredService<ILoggingOptions>(), dbContext);
		var interceptor = new ValidationSaveChangesInterceptor(sp,
			sp.GetRequiredService<ILogger<ValidationSaveChangesInterceptor>>(),
			sp.GetRequiredService<IOptionsMonitor<DatabaseEntitiesConfig>>());

		var intercept = () => interceptor.SavingChangesAsync(eventData, default).AsTask();
		var exceptionAssert = await intercept.Should()
			.ThrowExactlyAsync<AggregateException>();
		exceptionAssert.And.InnerExceptions
			.Should().AllBeAssignableTo<ValidationException>()
			.And.ContainSingle()
			.And.BeEquivalentTo(new object[]
			{
				new
				{
					Errors = new[]
					{
						new
						{
							AttemptedValue = true,
							PropertyName = nameof(UnitAuditableEntity.Invalid),
							Severity = Severity.Error,
							ErrorCode = "EqualValidator"
						}
					}
				}
			});
	}
}