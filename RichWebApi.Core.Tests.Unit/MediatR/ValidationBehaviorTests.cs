using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RichWebApi.Exceptions;
using RichWebApi.Tests;
using RichWebApi.Tests.DependencyInjection;
using RichWebApi.Tests.Logging;
using RichWebApi.Tests.Moq;
using Xunit.Abstractions;

namespace RichWebApi.Core.Tests.Unit.MediatR;

public class ValidationBehaviorTests : UnitTest
{
	private readonly DependencyContainerFixture _container;

	public ValidationBehaviorTests(ITestOutputHelper testOutputHelper, UnitDependencyContainerFixture container)
		: base(testOutputHelper)
		=> _container = container
			.WithXunitLogging(TestOutputHelper)
			.ConfigureServices(s => s.CollectCoreServicesFromAssembly(typeof(ValidationBehaviorTests).Assembly));

	[Fact]
	public async Task CallsValidator()
	{
		var request = new UnitRequest();
		var sp = _container
			.ReplaceWithMock<IValidator<UnitRequest>>(mock => mock.Setup(x
					=> x.ValidateAsync(It.Is<UnitRequest>(r => r == request),
						It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResult()))
			.BuildServiceProvider();
		var mediator = sp.GetRequiredService<IMediator>();

		await mediator.Send(request);

		var mock = sp.GetRequiredService<Mock<IValidator<UnitRequest>>>();
		mock.Verify(x
			=> x.ValidateAsync(It.Is<UnitRequest>(r => r == request),
				It.IsAny<CancellationToken>()), Times.Once());
	}

	[Fact]
	public async Task CallsHandlerIfValidRequest()
	{
		var request = new UnitRequest();
		var sp = _container
			.ReplaceWithMock<IRequestHandler<UnitRequest>>(mock
				=> mock.Setup(x => x.Handle(It.Is<UnitRequest>(r => r == request), It.IsAny<CancellationToken>()))
					.Returns(Task.CompletedTask)
					.Verifiable())
			.BuildServiceProvider();
		var mediator = sp.GetRequiredService<IMediator>();

		await mediator.Send(request);

		var mock = sp.GetRequiredService<Mock<IRequestHandler<UnitRequest>>>();
		mock.Verify(x => x.Handle(It.Is<UnitRequest>(r => r == request), It.IsAny<CancellationToken>()), Times.Once());
	}

	[Fact]
	public async Task ThrowsIfInvalidRequest()
	{
		var request = new UnitRequest(true);
		var sp = _container
			.ReplaceWithMock<IRequestHandler<UnitRequest>>(mock
				=> mock.Setup(x => x.Handle(It.Is<UnitRequest>(r => r == request), It.IsAny<CancellationToken>()))
					.Returns(Task.CompletedTask)
					.Verifiable())
			.BuildServiceProvider();
		var mediator = sp.GetRequiredService<IMediator>();

		var action = () => mediator.Send(request);
		await action.Should()
			.ThrowExactlyAsync<RichWebApiValidationException>();
	}

	public override async Task DisposeAsync()
	{
		await base.DisposeAsync();
		_container.Clear();
	}

	public record UnitRequest(bool Invalid = false) : IRequest
	{
		[UsedImplicitly]
		public class Validator : AbstractValidator<UnitRequest>
		{
			public Validator() => RuleFor(x => x.Invalid).Equal(false);
		}

		[UsedImplicitly]
		internal class UnitRequestHandler : IRequestHandler<UnitRequest>
		{
			public Task Handle(UnitRequest request, CancellationToken cancellationToken)
				=> Task.CompletedTask;
		}
	}
}