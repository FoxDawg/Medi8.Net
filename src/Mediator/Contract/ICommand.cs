namespace Mediator.Contract;

// Empty marker interface
#pragma warning disable CA1040
public interface ICommand : ICommand<NoResult>
{
}

// Empty marker interface
// ReSharper disable once UnusedTypeParameter
public interface ICommand<out TResponse> : IRequest
{
}

#pragma warning restore CA1040