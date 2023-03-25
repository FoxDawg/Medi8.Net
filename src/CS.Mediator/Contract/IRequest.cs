namespace CS.Mediator.Contract;

// Empty marker interface
#pragma warning disable CA1040
public interface IRequest
{
}

// Empty marker interface
public interface ICommand<out TResponse> : IRequest
{
}

// Empty marker interface
public interface IQuery<out TResponse> : IRequest
{
}

#pragma warning restore CA1040