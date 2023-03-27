using System.Threading.Tasks;
using Mediator.Pipeline;

namespace Mediator.Contract;

public interface IProcessor
{
    Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next);
}

// Marker interface
public interface IPreProcessor : IProcessor
{
}

// Marker interface
public interface IPostProcessor : IProcessor
{
}