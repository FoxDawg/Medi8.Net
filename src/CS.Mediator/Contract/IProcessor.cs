using System.Threading.Tasks;
using CS.Mediator.Pipeline;

namespace CS.Mediator.Contract;

public interface IProcessor
{
    Task InvokeAsync(ProcessingContext context, NextFilter nextFilter);
}

// Marker interface
public interface IPreProcessor : IProcessor
{
}

// Marker interface
public interface IPostProcessor : IProcessor
{
}