using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RichWebApi.Config;
using RichWebApi.Entities;
using RichWebApi.Exceptions;
using RichWebApi.Tests;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Logging;
using RichWebApi.Tests.Moq;
using Xunit.Abstractions;

namespace RichWebApi.Persistence.Internal;

public class EntityValidatorsProviderTests : UnitTest
{
	private readonly DependencyContainerFixture _container;

	public EntityValidatorsProviderTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container) :
		base(testOutputHelper)
	{
		_container = container;
	}

	[Fact]
	public void ThrowsIfValidationRequiredWithNoValidators()
	{
		_container.SetDatabaseEntitiesConfig(EntitiesValidationOption.Required);
		var sp = EnrichWithSharedServices(_container)
			.BuildServiceProvider();
		var getter = () => sp.GetRequiredService<IEntityValidatorsProvider>();
		getter.Should()
			.Throw<MissingEntitiesValidatorsException>()
			.Which.MissingTypes
			.Should()
			.Contain(new[] { typeof(ConfigurableEntity), typeof(IgnoredEntity) });
	}

	[Fact]
	public void NotThrowsIfValidationNotRequired()
	{
		_container.SetDatabaseEntitiesConfig(EntitiesValidationOption.None);
		var sp = EnrichWithSharedServices(_container)
			.BuildServiceProvider();
		var getter = () => sp.GetRequiredService<IEntityValidatorsProvider>();
		getter.Should().NotThrow();
	}

	[Fact]
	public void NotThrowsIfHaveAllValidators()
	{
		_container
			.SetDatabaseEntitiesConfig(EntitiesValidationOption.Required)
			.AddEmptyMockedService<IValidator<ConfigurableEntity>>()
			.AddEmptyMockedService<IValidator<IgnoredEntity>>();
		var sp = EnrichWithSharedServices(_container)
			.BuildServiceProvider();
		var getter = () => sp.GetRequiredService<IEntityValidatorsProvider>();
		getter.Should().NotThrow();
	}

	[Fact]
	public void ProvidesValidatorForEntity()
	{
		_container
			.SetDatabaseEntitiesConfig(EntitiesValidationOption.None)
			.AddEmptyMockedService<IValidator<ConfigurableEntity>>();
		var sp = EnrichWithSharedServices(_container)
			.BuildServiceProvider();
		var validatorsProvider = sp.GetRequiredService<IEntityValidatorsProvider>();
		var validator = validatorsProvider.GetAsyncValidator(sp, typeof(ConfigurableEntity));
		validator
			.Should()
			.NotBeNull();
	}

	[Fact]
	public async Task ProvidedValidatorCallsFluentValidationValidator()
	{
		_container
			.SetDatabaseEntitiesConfig(EntitiesValidationOption.None)
			.AddMockedService<IValidator<ConfigurableEntity>>(configure: (_, mock) => mock
				.Setup(x => x.ValidateAsync(It.IsAny<ConfigurableEntity>(), It.IsAny<CancellationToken>()))
				.Verifiable()
			);
		var sp = EnrichWithSharedServices(_container)
			.BuildServiceProvider();
		var validatorsProvider = sp.GetRequiredService<IEntityValidatorsProvider>();
		var validator = validatorsProvider.GetAsyncValidator(sp, typeof(ConfigurableEntity));
		await validator(new ConfigurableEntity(), default);

		var mock = sp.GetRequiredService<Mock<IValidator<ConfigurableEntity>>>();
		mock.Verify(x => x.ValidateAsync(It.IsAny<ConfigurableEntity>(), It.IsAny<CancellationToken>()), Times.Once());
	}

	[Fact]
	public Task ThrowsIfWrongEntityPassedToProvidedValidator()
	{
		_container
			.SetDatabaseEntitiesConfig(EntitiesValidationOption.None)
			.AddEmptyMockedService<IValidator<ConfigurableEntity>>();
		var sp = EnrichWithSharedServices(_container)
			.BuildServiceProvider();
		var validatorsProvider = sp.GetRequiredService<IEntityValidatorsProvider>();
		var validator = validatorsProvider.GetAsyncValidator(sp, typeof(ConfigurableEntity));
		var action = () => validator(new IgnoredEntity(), default);
		return action.Should()
			.ThrowExactlyAsync<InvalidCastException>("entity can't be explicitly casted to validator of given type");
	}

	[Fact]
	public void ThrowsIfTriesToProvideMissingValidator()
	{
		_container.SetDatabaseEntitiesConfig(EntitiesValidationOption.None);
		var sp = EnrichWithSharedServices(_container)
			.BuildServiceProvider();
		var validatorsProvider = sp.GetRequiredService<IEntityValidatorsProvider>();
		var entityType = typeof(ConfigurableEntity);
		var getter = () => validatorsProvider.GetAsyncValidator(sp, entityType);
		getter.Should()
			.ThrowExactly<MissingEntitiesValidatorsException>()
			.Which.MissingTypes.Should()
			.Contain(entityType);
	}

	private DependencyContainerFixture EnrichWithSharedServices(DependencyContainerFixture container)
	{
		var parts = new AppPartsCollection
		{
			new DatabaseUnitTestsPart()
		};
		return container
			.WithXunitLogging(TestOutputHelper)
			.WithTestScopeInMemoryDatabase(parts)
			.ConfigureServices(s => s.AddAppParts(parts));
	}

	public override async Task DisposeAsync()
	{
		await base.DisposeAsync();
		_container.ConfigureServices(s => s.Clear());
	}
}