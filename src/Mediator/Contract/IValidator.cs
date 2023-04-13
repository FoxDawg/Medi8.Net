using System.Threading.Tasks;

namespace Mediator.Contract;

public interface IValidator<TRequest>
    where TRequest : IRequest
{
    Task<Errors> ValidateAsync(IProcessingContext<TRequest> context);
}