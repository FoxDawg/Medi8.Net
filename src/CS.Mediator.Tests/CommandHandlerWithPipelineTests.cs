using CS.Mediator.Contract;
using CS.Mediator.Setup;
using CS.Mediator.Tests.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CS.Mediator.Tests;

public sealed class CommandHandlerWithPipelineTests
{
    [Fact]
    public async Task Handles_AuthorizedCommandWithResult_Successfully()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddHandler<CreateEntityCommand, CreateEntityCommand.CreateEntityCommandHandler>();
                config.AddPipelineFilter(_ => new AuthenticationFilter(true));
            });
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
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
    public async Task Handles_UnauthorizedCommandWithResult()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddHandler<CreateEntityCommand, CreateEntityCommand.CreateEntityCommandHandler>();
                config.AddPipelineFilter(_ => new AuthenticationFilter(false));
            });
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var command = new CreateEntityCommand("foobar");

        // Act
        var result = await mediator.HandleCommandAsync<CreateEntityCommand, CreateEntityCommand.EntityCreated>(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCode.Forbidden);
        result.ValidationResults.Should().ContainSingle();
    }

    [Fact]
    public async Task Exception_Propagates_BackToCaller()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddHandler<ThrowExceptionCommand, ThrowExceptionCommand.ThrowExceptionCommandHandler>();
                config.AddPipelineFilter(_ => new AuthenticationFilter(true));
            });
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var command = new ThrowExceptionCommand();

        var action = async () => await mediator.HandleCommandAsync(command, CancellationToken.None);

        await action.Should().ThrowAsync<ArithmeticException>();
    }

    private class AuthenticationFilter : IPipelineFilter
    {
        private readonly bool _isAuthenticated;

        public AuthenticationFilter(bool isAuthenticated)
        {
            _isAuthenticated = isAuthenticated;
        }

        public async Task InvokeAsync(ProcessingContext context, NextFilter next)
        {
            if (!_isAuthenticated)
            {
                context.WriteTo(StatusCode.Forbidden);
                return;
            }

            await next(context);
        }
    }
}