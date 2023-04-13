using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Mediator.Contract;
using Mediator.Setup;
using Mediator.Tests.Commands;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Mediator.Tests;

public sealed class CommandHandlerTests : IDisposable
{
    private readonly ServiceProvider serviceProvider;

    public CommandHandlerTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddHandler<CreateEntityCommand, CreateEntityCommand.CreateEntityCommandHandler>();
                config.AddValidator<CreateEntityCommand, CreateEntityCommand.CreateEntityCommandValidator>();
                config.AddHandler<ThrowExceptionCommand, ThrowExceptionCommand.ThrowExceptionCommandHandler>();
                config.AddHandler<DoWithoutResultCommand, DoWithoutResultCommand.DoWithoutResultCommandHandler>();
                config.AddValidator<DoWithoutResultCommand, DoWithoutResultCommand.DoWithoutCommandValidator>();
                config.AddHandler<ExecuteLongRunningTaskCommand, ExecuteLongRunningTaskCommand.ExecuteLongRunningTaskCommandHandler>();
            });
        this.serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public void Dispose()
    {
        this.serviceProvider.Dispose();
    }

    [Fact]
    public async Task LongRunningTask_CanBeCancelled()
    {
        // Arrange
        var mediator = this.serviceProvider.GetRequiredService<IMediator>();
        var command = new ExecuteLongRunningTaskCommand();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        // Act
        var result = await mediator.HandleCommandAsync(command, cts.Token).ConfigureAwait(false);

        // Assert
        using var scope = new AssertionScope();
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.CancellationRequested);
    }

    [Fact]
    public async Task Handles_CommandWithoutResult_Successfully()
    {
        // Arrange
        var mediator = this.serviceProvider.GetRequiredService<IMediator>();
        var command = new DoWithoutResultCommand("foobar");

        // Act
        var result = await mediator.HandleCommandAsync(command, CancellationToken.None).ConfigureAwait(false);

        // Assert
        using var scope = new AssertionScope();
        result.IsSuccessful.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Ok);
        result.Result.Should().BeOfType<NoResult>();
        result.Result.As<NoResult>().Should().NotBeNull();
    }

    [Fact]
    public async Task Handles_CommandWithResult_Successfully()
    {
        // Arrange
        var mediator = this.serviceProvider.GetRequiredService<IMediator>();
        var command = new CreateEntityCommand("foobar");

        // Act
        var result = await mediator.HandleCommandAsync<CreateEntityCommand, CreateEntityCommand.EntityCreated>(command, CancellationToken.None);

        // Assert
        using var scope = new AssertionScope();
        result.IsSuccessful.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Ok);
        result.Result.Should().BeOfType<CreateEntityCommand.EntityCreated>();
        result.Result.As<CreateEntityCommand.EntityCreated>().Should().NotBeNull();
    }

    [Fact]
    public async Task Validates_CommandWithoutResult_Successfully()
    {
        // Arrange
        var mediator = this.serviceProvider.GetRequiredService<IMediator>();
        var command = new DoWithoutResultCommand("invalid");

        // Act
        var result = await mediator.HandleCommandAsync(command, CancellationToken.None);

        // Assert
        using var scope = new AssertionScope();
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.ValidationFailed);
        result.Result.Should().BeNull();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public async Task Validates_CommandWithResult_Successfully()
    {
        // Arrange
        var mediator = this.serviceProvider.GetRequiredService<IMediator>();
        var command = new CreateEntityCommand("invalid");

        // Act
        var result = await mediator.HandleCommandAsync<CreateEntityCommand, CreateEntityCommand.EntityCreated>(command, CancellationToken.None);

        // Assert
        using var scope = new AssertionScope();
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.ValidationFailed);
        result.Result.Should().BeNull();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public async Task Exception_Propagates_BackToCaller()
    {
        var mediator = this.serviceProvider.GetRequiredService<IMediator>();

        var command = new ThrowExceptionCommand();

        var action = async () => await mediator.HandleCommandAsync(command, CancellationToken.None);

        await action.Should().ThrowAsync<ArithmeticException>();
    }
}