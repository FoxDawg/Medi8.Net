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

public class MiddlewareTests
{
    [Fact]
    public async Task OnSuccess_Middlewares_Are_Called_Once()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<PreProcessorCounter>();
        serviceCollection.AddSingleton<PostProcessorCounter>();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddHandler<DoWithoutResultCommand, DoWithoutResultCommand.DoWithoutResultCommandHandler>();
                config.AddValidator<DoWithoutResultCommand, DoWithoutResultCommand.DoWithoutCommandValidator>();
                config.AddPreExecutionMiddleware<PreMiddleware>();
                config.AddPostExecutionMiddleware<PostMiddleware>();
            });
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var result = await serviceProvider.GetRequiredService<IMediator>().HandleCommandAsync<DoWithoutResultCommand, NoResult>(new DoWithoutResultCommand("foo"), CancellationToken.None);

        using var scope = new AssertionScope();
        result.IsSuccessful.Should().BeTrue();
        serviceProvider.GetRequiredService<PreProcessorCounter>().Count.Should().Be(1);
        serviceProvider.GetRequiredService<PostProcessorCounter>().Count.Should().Be(1);
    }

    [Fact]
    public async Task OnValidationError_Middlewares_Are_Called()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<PreProcessorCounter>();
        serviceCollection.AddSingleton<PostProcessorCounter>();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddHandler<DoWithoutResultCommand, DoWithoutResultCommand.DoWithoutResultCommandHandler>();
                config.AddValidator<DoWithoutResultCommand, DoWithoutResultCommand.DoWithoutCommandValidator>();
                config.AddPreExecutionMiddleware<PreMiddleware>();
                config.AddPostExecutionMiddleware<PostMiddleware>();
            });
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var result = await serviceProvider.GetRequiredService<IMediator>().HandleCommandAsync<DoWithoutResultCommand, NoResult>(new DoWithoutResultCommand("invalid"), CancellationToken.None);

        using var scope = new AssertionScope();
        result.IsSuccessful.Should().BeFalse();
        serviceProvider.GetRequiredService<PreProcessorCounter>().Count.Should().Be(1);
        serviceProvider.GetRequiredService<PostProcessorCounter>().Count.Should().Be(0);
    }

    private class PreMiddleware : IPreProcessor
    {
        public async Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next)
            where TRequest : IRequest
        {
            var counter = context.GetRequiredService<PreProcessorCounter>();
            counter.Increment();
            await next(context);
        }
    }

    private class PreProcessorCounter
    {
        public int Count { get; private set; }

        public void Increment()
        {
            this.Count++;
        }
    }

    private class PostMiddleware : IPostProcessor
    {
        public async Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next)
            where TRequest : IRequest
        {
            var counter = context.GetRequiredService<PostProcessorCounter>();
            counter.Increment();
            await next(context);
        }
    }

    private class PostProcessorCounter
    {
        public int Count { get; private set; }

        public void Increment()
        {
            this.Count++;
        }
    }
}