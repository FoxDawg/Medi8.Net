using System;
using System.Threading.Tasks;
using Mediator.Contract;

namespace Mediator.Tests.Commands;

public record ExecuteLongRunningTaskCommand : ICommand
{
    public class ExecuteLongRunningTaskCommandHandler : ICommandHandler<ExecuteLongRunningTaskCommand>
    {
        public async Task HandleAsync(IProcessingContext<ExecuteLongRunningTaskCommand> context)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), context.Token).ConfigureAwait(false);
        }
    }
}