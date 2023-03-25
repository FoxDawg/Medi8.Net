namespace CS.Mediator.Contract;

// Empty marker interface
#pragma warning disable CA1040
public interface IRequest
{
}

// Empty marker interface
// ReSharper disable once UnusedTypeParameter
public interface ICommand<out TResponse> : IRequest
{
}

// Empty marker interface
// ReSharper disable once UnusedTypeParameter
public interface IQuery<out TResponse> : IRequest
{
}

#pragma warning restore CA1040