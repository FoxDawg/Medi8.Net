using CS.Mediator.Contract;
using CS.Mediator.Setup;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CS.Mediator.Tests;

public sealed class PipelineTests 
{
    [Fact]
    public async Task PipelineWith_Three_Filters_Works_As_Expected()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddPipelineFilter<TestFilter>(_ => new TestFilter(StatusCode.Unauthorized));
                config.AddPipelineFilter<TestFilter>(_ => new TestFilter(StatusCode.BadRequest));
                config.AddPipelineFilter<TestFilter>(_ => new TestFilter(StatusCode.InternalError));
            });
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var pipelineBuilder = serviceProvider.GetRequiredService<PipelineBuilder>();
        var pipelineStart = pipelineBuilder.Build();
        var context = new ProcessingContext();

        // Act
        await pipelineStart.Invoke(context);

        // Assert
        context.StatusCode.Should().Be(StatusCode.InternalError);
    }

    [Fact]
    public async Task PipelineWithoutFilters_Works_As_Expected()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator();
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var pipelineBuilder = serviceProvider.GetRequiredService<PipelineBuilder>();
        var pipelineStart = pipelineBuilder.Build();
        var context = new ProcessingContext();

        // Act
        await pipelineStart.Invoke(context);

        // Assert
        context.StatusCode.Should().Be(StatusCode.Ok);
    }

    private class TestFilter : IPipelineFilter
    {
        private readonly StatusCode _order;

        public TestFilter(StatusCode order)
        {
            _order = order;
        }

        public async Task InvokeAsync(ProcessingContext context, NextFilter next)
        {
            context.WriteTo(_order);
            await next(context);
        }
    }
}