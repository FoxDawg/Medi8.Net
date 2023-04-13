using System;
using System.Threading.Tasks;
using Mediator.Contract;

namespace Mediator.Tests.Commands;

public record ExecuteLongRunningTaskCommand : ICommand<NoResult>
{
public class ExecuteLongRunningTaskCommandHandler : ICommandHandler<ExecuteLongRunningTaskCommand, NoResult>
{
    public async Task<NoResult> HandleAsync(IProcessingContext<ExecuteLongRunningTaskCommand> context)
    {
        await Task.Delay(TimeSpan.FromSeconds(10), context.Token).ConfigureAwait(false);
        return NoResult.Create();
    }
}
}