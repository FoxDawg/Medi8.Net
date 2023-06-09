using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Mediator.Contract;
using Mediator.Setup;
using Mediator.Tests.Queries;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Mediator.Tests;

public sealed class QueryHandlerTests : IDisposable
{
    private readonly ServiceProvider serviceProvider;

    public QueryHandlerTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMediator(
            config =>
            {
                config.AddHandler<GetEntityQuery, GetEntityQuery.GetEntityQueryHandler>();
                config.AddValidator<GetEntityQuery, GetEntityQuery.GetEntityQueryValidator>();
            });
        this.serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public void Dispose()
    {
        this.serviceProvider.Dispose();
    }

    [Fact]
    public async Task Handles_Query_Successfully()
    {
        // Arrange
        var mediator = this.serviceProvider.GetRequiredService<IMediator>();
        var query = new GetEntityQuery(GetEntityQuery.ValidId);

        // Act
        var result = await mediator.HandleQueryAsync<GetEntityQuery, GetEntityQuery.Entity>(query, CancellationToken.None);

        // Assert
        using var scope = new AssertionScope();
        result.IsSuccessful.Should().BeTrue();
        result.Status.Should().Be(Status.Ok);
        result.Result.Should().BeOfType<GetEntityQuery.Entity>();
        result.Result.As<GetEntityQuery.Entity>().Should().NotBeNull();
        result.Result.As<GetEntityQuery.Entity>().Id.Should().Be(query.Id);
    }

    [Fact]
    public async Task Handles_QueryWithNullResult_Successfully()
    {
        // Arrange
        var mediator = this.serviceProvider.GetRequiredService<IMediator>();
        var query = new GetEntityQuery(GetEntityQuery.NonExistingId);

        // Act
        var result = await mediator.HandleQueryAsync<GetEntityQuery, GetEntityQuery.Entity>(query, CancellationToken.None);

        // Assert
        using var scope = new AssertionScope();
        result.IsSuccessful.Should().BeTrue();
        result.Status.Should().Be(Status.Ok);
        result.Result.Should().BeNull();
    }

    [Fact]
    public async Task Validates_Query_Successfully()
    {
        // Arrange
        var mediator = this.serviceProvider.GetRequiredService<IMediator>();
        var query = new GetEntityQuery(GetEntityQuery.InvalidId);

        // Act
        var result = await mediator.HandleQueryAsync<GetEntityQuery, GetEntityQuery.Entity>(query, CancellationToken.None);

        // Assert
        using var scope = new AssertionScope();
        result.IsSuccessful.Should().BeFalse();
        result.Status.Should().Be(Status.ValidationFailed);
        result.Result.Should().BeNull();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public async Task Exception_Propagates_BackToCaller()
    {
        // Arrange
        var mediator = this.serviceProvider.GetRequiredService<IMediator>();
        var query = new GetEntityQuery(GetEntityQuery.ExceptionId);

        var action = async () => await mediator.HandleQueryAsync<GetEntityQuery, GetEntityQuery.Entity>(query, CancellationToken.None);

        await action.Should().ThrowAsync<ArithmeticException>();
    }
}