using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
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
			.ReplaceWithMock<IValidator<UnitRequest>>(mock => mock.ValidateAsync(Arg.Is<UnitRequest>(r => r == request),
						Arg.Any<CancellationToken>())
				.Returns(new ValidationResult()))
			.BuildServiceProvider();
		var mediator = sp.GetRequiredService<IMediator>();

		await mediator.Send(request);

		var mock = sp.GetRequiredService<IValidator<UnitRequest>>();
		await mock.Received(1).ValidateAsync(Arg.Is<UnitRequest>(r => r == request),
				Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task CallsHandlerIfValidRequest()
	{
		var request = new UnitRequest();
		var sp = _container
			.ReplaceWithMock<IRequestHandler<UnitRequest>>(mock
				=> mock.Handle(Arg.Is<UnitRequest>(r => r == request), Arg.Any<CancellationToken>())
					.Returns(Task.CompletedTask))
			.BuildServiceProvider();
		var mediator = sp.GetRequiredService<IMediator>();

		await mediator.Send(request);

		var mock = sp.GetRequiredService<IRequestHandler<UnitRequest>>();
		await mock.Received(1).Handle(Arg.Is<UnitRequest>(r => r == request), Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task ThrowsIfInvalidRequest()
	{
		var request = new UnitRequest(true);
		var sp = _container
			.ReplaceWithMock<IRequestHandler<UnitRequest>>(mock
				=> mock.Handle(Arg.Is<UnitRequest>(r => r == request), Arg.Any<CancellationToken>())
					.Returns(Task.CompletedTask))
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