using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
                config.AddValidator<CreateEntityCommand,CreateEntityCommand.CreateEntityCommandValidator>();
                config.AddHandler<ThrowExceptionCommand, ThrowExceptionCommand.ThrowExceptionCommandHandler>();
                config.AddHandler<DoWithoutResultCommand, DoWithoutResultCommand.DoWithoutResultCommandHandler>();
                config.AddValidator<DoWithoutResultCommand, DoWithoutResultCommand.DoWithoutCommandValidator>();
            });
        this.serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public void Dispose()
    {
        this.serviceProvider.Dispose();
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
        result.IsSuccessful.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Ok);
        result.Result.Should().BeOfType<EmptyResult>();
        result.Result.As<EmptyResult>().Should().NotBeNull();
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
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.ValidationFailed);
        result.Result.Should().BeNull();
        result.ProcessingResults.Should().ContainSingle();
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
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.ValidationFailed);
        result.Result.Should().BeNull();
        result.ProcessingResults.Should().ContainSingle();
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