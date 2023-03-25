using System.Threading.Tasks;
using CS.Mediator.Contract;
using CS.Mediator.Pipeline;
using CS.Mediator.Setup;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CS.Mediator.Tests;

public sealed class PipelineTests
{
    [Fact]
    public async Task PipelineWith_Three_PreFilters_Works_As_Expected()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddToPipeline(_ => new PreFilter(StatusCode.Unauthorized));
                config.AddToPipeline(_ => new PreFilter(StatusCode.BadRequest));
                config.AddToPipeline(_ => new PreFilter(StatusCode.InternalError));
            });
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var pipelineBuilder = serviceProvider.GetRequiredService<PipelineBuilder>();
        var pipelineStart = pipelineBuilder.BuildPreProcessorPipeline();
        var context = new ProcessingContext();

        // Act
        await pipelineStart.Invoke(context);

        // Assert
        context.StatusCode.Should().Be(StatusCode.InternalError);
    }

    [Fact]
    public async Task PipelineWith_TwoPostFilters_Works_As_Expected()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddToPipeline(_ => new PostFilter(StatusCode.BadRequest));
                config.AddToPipeline(_ => new PostFilter(StatusCode.Unauthorized));
            });
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var pipelineBuilder = serviceProvider.GetRequiredService<PipelineBuilder>();
        var pipelineStart = pipelineBuilder.BuildPostProcessorPipeline();
        var context = new ProcessingContext();

        // Act
        await pipelineStart.Invoke(context);

        // Assert
        context.StatusCode.Should().Be(StatusCode.Unauthorized);
    }

    [Fact]
    public async Task PipelineWithoutFilters_Works_As_Expected()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var pipelineBuilder = serviceProvider.GetRequiredService<PipelineBuilder>();
        var pipelineStart = pipelineBuilder.BuildPreProcessorPipeline();
        var context = new ProcessingContext();

        // Act
        await pipelineStart.Invoke(context);

        // Assert
        context.StatusCode.Should().Be(StatusCode.Ok);
    }

    private class PreFilter : IPreProcessor
    {
        private readonly StatusCode order;

        public PreFilter(StatusCode order)
        {
            this.order = order;
        }

        public async Task InvokeAsync(ProcessingContext context, NextFilter nextFilter)
        {
            context.WriteTo(this.order);
            await nextFilter(context);
        }
    }

    private class PostFilter : IPostProcessor
    {
        private readonly StatusCode order;

        public PostFilter(StatusCode order)
        {
            this.order = order;
        }

        public async Task InvokeAsync(ProcessingContext context, NextFilter nextFilter)
        {
            context.WriteTo(this.order);
            await nextFilter(context);
        }
    }
}