## Medi8.Net

Medi8.Net is a simple yet fast mediator implementation in .NET 7.
It makes it easy to design your application according to CQRS principles and slice your solution into vertical and feature-based parts.

Medi8.Net is Apache 2.0 licensed.

### Getting started

#### Step 1 - Installing the package
Using .NET CLI
```bash
dotnet add package Medi8.Net --version 1.0.0
```

Using the package manager console
```bash
NuGet\Install-Package Medi8.Net -Version 1.0.0
```

#### Step 2 - Adding the necessary infrastructure in your `Program.cs`

```csharp
serviceCollection.AddMediator(cfg =>
{
    cfg.AddHandler<MyQuery, MyQueryHandler>();
    cfg.AddHandler<MyCommand, MyCommandHandler>();
});
```

#### Step 3 - Using Medi8.Net in your code, e.g., controllers

After injecting `IMediator` into your business logic, you are ready to go:
```csharp
var result = await this.mediator.HandleQueryAsync<MyQuery, MyResult?>(new MyQuery(...), CancellationToken.None);
```

The result object will contain the result of the query/command and also information about the operation itself.
Any exceptions during the handling of the query will be directly propagated back to the caller.

#### Step 4 - Implementing queries/commands and handlers:
```csharp
public record MyQuery(string Name) : IQuery<MyResult?>
{
    public class MyQueryHandler : IQueryHandler<MyQuery, MyResult?>
    {
        public Task<MyResult?> HandleAsync(IProcessingContext<MyQuery> context)
        {
            // perform the actual handling of the query and return the result
        }
    }
}

public record MyCommandWithResult : ICommand<MyResult?>
{
    public class MyCommandWithResultHandler : ICommandHandler<MyCommand, MyResult?>
    {
        public Task<MyResult?> HandleAsync(IProcessingContext<MyCommand> context)
        {
            // perform the actual handling of the query and return the result
        }
    }
}

public record MyCommand : ICommand
{
    public class MyCommandHandler : ICommandHandler<MyCommand>
    {
        public Task HandleAsync(IProcessingContext<MyCommand> context)
        {
            // perform the actual handling of the query and return the result
        }
    }
}
```

### Advanced scenarios


#### Validation
Validation in Medi8.Net is possible by implementing an interface `IValidateRequest<TRequest>` in a class.
This interface will automatically be picked up by the middleware pipeline:
```csharp
builder.Services.AddMediator(cfg =>
{
    cfg.AddHandler<FindProductByIdQuery, FindProductByIdQuery.FindProductByIdQueryHandler>();
    cfg.AddHandler<AddProductCommand, AddProductCommand.AddProductCommandHandler>();
    cfg.AddValidator<AddProductCommand, AddProductCommand.AddProductValidator>();
    cfg.AddValidator<FindProductByIdQuery, FindProductByIdQuery.FindProductByIdQueryValidator>();
});
```
and an implementation
```csharp
public class FindProductByIdQueryValidator : IValidateRequest<FindProductByIdQuery>
{
    public Task<Errors> ValidateAsync(IProcessingContext<FindProductByIdQuery> context)
    {
        // do implementation here
        // ...
        return Task.FromResult(Errors.Empty);
    }
}
```

#### Middleware
Medi8.Net supports full pipeline processing for pre- and post-processing of a request.
You can implement custom filters using `IPreProcessor` and `IPostProcessor` and add them to your
code as follows:

```csharp
serviceCollection.AddMediator(
    cfg =>
    {
        cfg.AddHandler<MyQuery, MyQueryHandler>();
        cfg.AddPreExecutionMiddleware<MyPreFilter>();
        cfg.AddPostExecutionMiddleware<MyPreFilter>();
    });
```

The filters will be executed _in order of their registration_.

If filters rely on results from previous filters or if you need to transport custom payloads through your pipeline,
you can access respective methods on the `ProcessingContext`:
```csharp
public class PreFilter : IPreProcessor
{
    public async Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next)
    {
        context.TryAddPayload("MyKey", 42);
        await next(context);
    }
}
```

Intercepting the pipeline through filters is recommended to be done using status codes and adding errors to the context:
```csharp
public class PreFilter : IPreProcessor
{
    public async Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next)
    {
        if (somethingWentWrong)
        {
            context.WriteTo(new Errors("MyFilter", "Failed because of reason..."));
            context.WriteTo(StatusCode.PipelineFailed);
        }
        else
        {
            await next(context);
        }
    }
}
```

If your filters need dependencies, this will not be possible through ctor injection. Instead, you can resolve your 
dependencies from the scope provided by the context:
```csharp
public async Task InvokeAsync<TRequest>(ProcessingContext<TRequest> context, Next<TRequest> next)
{
    var statusCode = context.GetRequiredService<StatusCodeProvider>().GetAndIncrement();
    context.WriteTo(statusCode);
    await next(context);
}
```

### Samples

Please refer to the [samples](https://github.com/FoxDawg/Medi8.Net/tree/develop/samples/Sample.WebApi) section for a simple
but full example of a small web application.

### Contribution

Feel free to raise issues and discussions in this repository for bugs and feature requests.