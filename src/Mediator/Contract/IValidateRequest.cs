using System.Threading.Tasks;

namespace Mediator.Contract;

public interface IValidateRequest<TRequest>
{
    Task<ProcessingResults> ValidateAsync(ProcessingContext<TRequest> context);
}