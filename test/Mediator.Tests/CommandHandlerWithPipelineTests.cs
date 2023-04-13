using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Mediator.Contract;
using Mediator.Pipeline;
using Mediator.Setup;
using Mediator.Tests.Commands;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Mediator.Tests;

public sealed class CommandHandlerWithPipelineTests
{
    [Fact]
    public async Task PostFilter_Handles_Command()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<Func<bool>>(_ => () => true);
        serviceCollection.AddMediator(
            config =>
            {
                config.AddHandler<CreateEntityCommand, CreateEntityCommand.CreateEntityCommandHandler>();
                config.AddValidator<CreateEntityCommand, CreateEntityCommand.CreateEntityCommandValidator>();
                config.AddPostExecutionMiddleware<PostFilter>();
            });
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var command = new CreateEntityCommand("foobar");

        // Act
        var result = await mediator.HandleCommandAsync<CreateEntityCommand, CreateEntityCommand.EntityCreated>(command, CancellationToken.None);

        // Assert
        using var scope = new AssertionScope();
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.PipelineFailed);
        result.Result.Should().BeNull();
    }

    [Fact]
    public async Task Handles_AuthorizedCommandWithResult_Successfully()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<Func<bool>>(_ => () => true);
        serviceCollection.AddMediator(
            config =>
            {
                config.AddHandler<CreateEntityCommand, CreateEntityCommand.CreateEntityCommandHandler>();
                config.AddPreExecutionMiddleware<AuthenticationFilter>();
            });
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
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
    public async Task AfterCancellation_NotEvenFirstFilterIsExecuted()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<StatusCodeProvider>();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddHandler<CreateEntityCommand, CreateEntityCommand.CreateEntityCommandHandler>();
                config.AddPreExecutionMiddleware<ProcessingCounter>();
            });
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var command = new CreateEntityCommand("foobar");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var result = await mediator.HandleCommandAsync<CreateEntityCommand, CreateEntityCommand.EntityCreated>(command, cts.Token);

        // Assert
        using var scope = new AssertionScope();
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.CancellationRequested);
        result.Result.Should().BeNull();
        serviceProvider.GetRequiredService<StatusCodeProvider>().StatusCode.Should().Be(0);
    }

    [Fact]
    public async Task AfterCancellation_PipelineIsNotContinued()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddHandler<CreateEntityCommand, CreateEntityCommand.CreateEntityCommandHandler>();
                config.AddPreExecutionMiddleware<LongRunningPreProcessor>();
            });
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var command = new CreateEntityCommand("foobar");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var result = await mediator.HandleCommandAsync<CreateEntityCommand, CreateEntityCommand.EntityCreated>(command, cts.Token);

        // Assert
        using var scope = new AssertionScope();
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.CancellationRequested);
        result.Result.Should().BeNull();
    }

    [Fact]
    public async Task Handles_UnauthorizedCommandWithResult()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<Func<bool>>(_ => () => false);
        serviceCollection.AddMediator(
            config =>
            {
                config.AddHandler<CreateEntityCommand, CreateEntityCommand.CreateEntityCommandHandler>();
                config.AddPreExecutionMiddleware<AuthenticationFilter>();
            });
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var command = new CreateEntityCommand("foobar");

        // Act
        var result = await mediator.HandleCommandAsync<CreateEntityCommand, CreateEntityCommand.EntityCreated>(command, CancellationToken.None);

        // Assert
        using var scope = new AssertionScope();
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Forbidden);
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public async Task Exception_Propagates_BackToCaller()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<Func<bool>>(_ => () => true);
        serviceCollection.AddMediator(
            config =>
            {
                config.AddHandler<ThrowExceptionCommand, ThrowExceptionCommand.ThrowExceptionCommandHandler>();
                config.AddPreExecutionMiddleware<AuthenticationFilter>();
            });
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var command = new ThrowExceptionCommand();

        var action = async () => await mediator.HandleCommandAsync(command, CancellationToken.None);

        await action.Should().ThrowAsync<ArithmeticException>();
    }

    private class AuthenticationFilter : IPreProcessor
    {
        public async Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next)
            where TRequest : IRequest
        {
            var isAuthenticated = context.GetRequiredService<Func<bool>>()();
            if (!isAuthenticated)
            {
                context.WriteTo(StatusCodes.Forbidden);
                context.WriteTo(new Error("Auth", "Not authenticated."));
                return;
            }

            await next(context);
        }
    }

    private class PostFilter : IPostProcessor
    {
        public Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next)
            where TRequest : IRequest
        {
            context.WriteTo(StatusCodes.PipelineFailed);
            return Task.CompletedTask;
        }
    }

    private class LongRunningPreProcessor : IPreProcessor
    {
        public async Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next)
            where TRequest : IRequest
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }

    private class ProcessingCounter : IPreProcessor
    {
        public async Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next)
            where TRequest : IRequest
        {
            _ = context.GetRequiredService<StatusCodeProvider>().GetAndIncrement();
            await next(context);
        }
    }

    private class StatusCodeProvider
    {
        public int StatusCode { get; private set; }

        public int GetAndIncrement()
        {
            return ++this.StatusCode;
        }
    }
}