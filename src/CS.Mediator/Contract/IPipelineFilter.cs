namespace CS.Mediator.Contract;

public interface IPipelineFilter
{
    Task InvokeAsync(ProcessingContext context, NextFilter next);
}