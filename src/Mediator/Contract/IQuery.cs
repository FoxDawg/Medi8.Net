namespace Mediator.Contract;

// Empty marker interface
// ReSharper disable once UnusedTypeParameter
#pragma warning disable CA1040
public interface IQuery<out TResponse> : IRequest
{
}
#pragma warning restore CA1040