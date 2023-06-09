using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Mediator.Contract;
using Mediator.Pipeline;
using Mediator.Setup;
using Mediator.Tests.Commands;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Mediator.Tests;

public sealed class PipelineTests
{
    [Fact]
    public async Task PipelineWith_Three_PreFilters_Works_As_Expected()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<StatusCodeProvider>();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddPreExecutionMiddleware<PreFilter>();
                config.AddPreExecutionMiddleware<PreFilter>();
            });
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var pipelineBuilder = serviceProvider.GetRequiredService<PipelineBuilder>();
        var pipelineStart = pipelineBuilder.BuildPreProcessorPipeline<DoWithoutResultCommand>();
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = new ProcessingContext<DoWithoutResultCommand>(scope, CancellationToken.None);

        // Act
        await pipelineStart.Invoke(context);

        // Assert
        context.Status.Value.Should().Be(2);
    }

    [Fact]
    public async Task PipelineWith_TwoPostFilters_Works_As_Expected()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<StatusCodeProvider>();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddPostExecutionMiddleware<PostFilter>();
                config.AddPostExecutionMiddleware<PostFilter>();
            });
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var pipelineBuilder = serviceProvider.GetRequiredService<PipelineBuilder>();
        var pipelineStart = pipelineBuilder.BuildPostProcessorPipeline<DoWithoutResultCommand>();
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = new ProcessingContext<DoWithoutResultCommand>(scope, CancellationToken.None);

        // Act
        await pipelineStart.Invoke(context);

        // Assert
        context.Status.Value.Should().Be(2);
    }

    [Fact]
    public async Task PipelineWithoutFilters_Works_As_Expected()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var pipelineBuilder = serviceProvider.GetRequiredService<PipelineBuilder>();
        var pipelineStart = pipelineBuilder.BuildPreProcessorPipeline<DoWithoutResultCommand>();
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = new ProcessingContext<DoWithoutResultCommand>(scope, CancellationToken.None);

        // Act
        await pipelineStart.Invoke(context);

        // Assert
        context.Status.Should().Be(Status.Ok);
    }

    private class PreFilter : IPreProcessor
    {
        public async Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next)
            where TRequest : IRequest
        {
            var statusCode = context.GetRequiredService<StatusCodeProvider>().GetAndIncrement();
            context.WriteTo(statusCode);
            await next(context);
        }
    }

    private class PostFilter : IPostProcessor
    {
        public async Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next)
            where TRequest : IRequest
        {
            var statusCode = context.GetRequiredService<StatusCodeProvider>().GetAndIncrement();
            context.WriteTo(statusCode);
            await next(context);
        }
    }

    private class StatusCodeProvider
    {
        public record MyStatus(int Code) : Status(Code);
        public int StatusCode { get; private set; }

        public Status GetAndIncrement()
        {
            return new MyStatus(++this.StatusCode);
        }
    }
}