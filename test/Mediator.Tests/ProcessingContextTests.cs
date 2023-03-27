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

public class ProcessingContextTests
{
    [Fact]
    public async Task PostFilter_CanAccess_Payload()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddHandler<CreateEntityCommand, CreateEntityCommand.CreateEntityCommandHandler>();
                config.AddPreExecutionMiddleware<PreFilter>();
                config.AddPostExecutionMiddleware<PostFilter>();
            });
        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var command = new CreateEntityCommand("foobar");

        // Act
        var result = await mediator.HandleCommandAsync<CreateEntityCommand, CreateEntityCommand.EntityCreated>(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Ok);
    }

    private class PreFilter : IPreProcessor
    {
        public async Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next)
        {
            context.TryAddPayload("MyKey", 42);
            await next(context);
        }
    }

    private class PostFilter : IPostProcessor
    {
        public async Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next)
        {
            if (context.TryGetPayload("MyKey") is 42)
            {
                await next(context);
            }
            else
            {
                context.WriteTo(StatusCodes.PipelineFailed);
            }
        }
    }
}