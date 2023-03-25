using System.Threading.Tasks;

namespace CS.Mediator.Contract;

public interface IPipelineFilter
{
    Task InvokeAsync(ProcessingContext context, NextFilter nextFilter);
}