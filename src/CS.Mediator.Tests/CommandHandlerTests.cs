using CS.Mediator.Contract;
using CS.Mediator.Setup;
using CS.Mediator.Tests.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CS.Mediator.Tests;

public sealed class CommandHandlerTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public CommandHandlerTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddHandler<CreateEntityCommand, CreateEntityCommand.CreateEntityCommandHandler>();
                config.AddHandler<ThrowExceptionCommand, ThrowExceptionCommand.ThrowExceptionCommandHandler>();
                config.AddHandler<DoWithoutResultCommand, DoWithoutResultCommand.DoWithoutResultCommandHandler>();
            });
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }

    [Fact]
    public async Task Handles_CommandWithoutResult_Successfully()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var command = new DoWithoutResultCommand("foobar");

        // Act
        var result = await mediator.HandleCommandAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCode.Ok);
        result.Result.Should().BeOfType<EmptyResult>();
        result.Result.As<EmptyResult>().Should().NotBeNull();
    }

    [Fact]
    public async Task Handles_CommandWithResult_Successfully()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var command = new CreateEntityCommand("foobar");

        // Act
        var result = await mediator.HandleCommandAsync<CreateEntityCommand, CreateEntityCommand.EntityCreated>(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCode.Ok);
        result.Result.Should().BeOfType<CreateEntityCommand.EntityCreated>();
        result.Result.As<CreateEntityCommand.EntityCreated>().Should().NotBeNull();
    }

    [Fact]
    public async Task Validates_CommandWithoutResult_Successfully()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var command = new DoWithoutResultCommand("invalid");

        // Act
        var result = await mediator.HandleCommandAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCode.BadRequest);
        result.Result.Should().BeNull();
        result.ValidationResults.Should().ContainSingle();
    }

    [Fact]
    public async Task Validates_CommandWithResult_Successfully()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var command = new CreateEntityCommand("invalid");

        // Act
        var result = await mediator.HandleCommandAsync<CreateEntityCommand, CreateEntityCommand.EntityCreated>(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCode.BadRequest);
        result.Result.Should().BeNull();
        result.ValidationResults.Should().ContainSingle();
    }

    [Fact]
    public async Task Exception_Propagates_BackToCaller()
    {
        var mediator = _serviceProvider.GetRequiredService<IMediator>();

        var command = new ThrowExceptionCommand();

        var action = async () => await mediator.HandleCommandAsync(command, CancellationToken.None);

        await action.Should().ThrowAsync<ArithmeticException>();
    }
}