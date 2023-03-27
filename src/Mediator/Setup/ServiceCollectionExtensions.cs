using System;
using Mediator.Contract;
using Mediator.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator.Setup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services, Action<MediatorConfigurator>? configure = default)
    {
        services.AddScoped<IMediator, RequestProcessor>();
        services.AddScoped<PipelineBuilder>();
        services.AddSingleton<PipelineCache>();

        var configurator = new MediatorConfigurator(services);
        configure?.Invoke(configurator);
        configurator.AddPreExecutionMiddleware<AsyncValidationMiddleware>();
        services.AddSingleton(_ => configurator.Build());
        return services;
    }
}