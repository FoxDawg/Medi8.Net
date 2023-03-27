using System.Threading.Tasks;

namespace Mediator.Contract;

public interface IValidateRequest<TRequest>
{
    Task<Errors> ValidateAsync(ProcessingContext<TRequest> context);
}