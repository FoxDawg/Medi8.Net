using System.Threading.Tasks;

namespace Mediator.Pipeline;

public delegate Task Next<TRequest>(ProcessingContext<TRequest> context);