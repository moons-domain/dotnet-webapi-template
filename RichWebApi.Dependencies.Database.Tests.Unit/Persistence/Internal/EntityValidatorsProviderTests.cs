using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RichWebApi.Config;
using RichWebApi.Exceptions;
using RichWebApi.Persistence.Internal;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Entities;
using RichWebApi.Tests.Logging;
using RichWebApi.Tests.Moq;
using Xunit.Abstractions;

namespace RichWebApi.Tests.Persistence.Internal;

public class EntityValidatorsProviderTests : UnitTest
{
	private readonly DependencyContainerFixture _container;

	public EntityValidatorsProviderTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container) :
		base(testOutputHelper)
	{
		var parts = new AppPartsCollection
		{
			new DatabaseUnitTestsPart()
		};
		_container = container
			.WithXunitLogging(TestOutputHelper)
			.WithTestScopeInMemoryDatabase(parts)
			.ConfigureServices(s => s.AddAppParts(parts));
	}

	[Fact]
	public void ThrowsIfValidationRequiredWithNoValidators()
	{
		var sp = _container
			.SetDatabaseEntitiesConfig(EntitiesValidationOption.Required)
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
		var sp = _container
			.SetDatabaseEntitiesConfig(EntitiesValidationOption.None)
			.BuildServiceProvider();
		var getter = () => sp.GetRequiredService<IEntityValidatorsProvider>();
		getter.Should().NotThrow();
	}

	[Fact]
	public void NotThrowsIfHaveAllValidators()
	{
		var sp = _container
			.SetDatabaseEntitiesConfig(EntitiesValidationOption.Required)
			.ReplaceWithEmptyMock<IValidator<ConfigurableEntity>>()
			.ReplaceWithEmptyMock<IValidator<IgnoredEntity>>()
			.BuildServiceProvider();
		var getter = () => sp.GetRequiredService<IEntityValidatorsProvider>();
		getter.Should().NotThrow();
	}

	[Fact]
	public void ProvidesValidatorForEntity()
	{
		var sp = _container
			.SetDatabaseEntitiesConfig(EntitiesValidationOption.None)
			.ReplaceWithEmptyMock<IValidator<ConfigurableEntity>>()
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
		var sp = _container
			.SetDatabaseEntitiesConfig(EntitiesValidationOption.None)
			.ReplaceWithMock<IValidator<ConfigurableEntity>>(mock => mock
				.Setup(x => x.ValidateAsync(It.IsAny<ConfigurableEntity>(), It.IsAny<CancellationToken>()))
				.Verifiable())
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
		var sp = _container
			.SetDatabaseEntitiesConfig(EntitiesValidationOption.None)
			.ReplaceWithEmptyMock<IValidator<ConfigurableEntity>>()
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
		var sp = _container
			.SetDatabaseEntitiesConfig(EntitiesValidationOption.None)
			.BuildServiceProvider();
		var validatorsProvider = sp.GetRequiredService<IEntityValidatorsProvider>();
		var entityType = typeof(ConfigurableEntity);
		var getter = () => validatorsProvider.GetAsyncValidator(sp, entityType);
		getter.Should()
			.ThrowExactly<MissingEntitiesValidatorsException>()
			.Which.MissingTypes.Should()
			.Contain(entityType);
	}

	public override async Task DisposeAsync()
	{
		await base.DisposeAsync();
		_container.Clear();
	}
}