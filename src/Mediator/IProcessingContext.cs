using System.Collections.Generic;
using System.Threading;
using Mediator.Contract;

namespace Mediator;

public interface IProcessingContext<TRequest>
    where TRequest : IRequest
{
    bool IsValid { get; }
    TRequest Request { get; }
    CancellationToken Token { get; }
    object? TryGetPayload(string key);
    bool TryAddPayload(string key, object payload);
    void WriteTo(IEnumerable<Error> results);
    void WriteTo(Error result);
}
