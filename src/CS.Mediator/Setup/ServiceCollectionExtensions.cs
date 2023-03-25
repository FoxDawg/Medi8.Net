using System;
using CS.Mediator.Contract;
using Microsoft.Extensions.DependencyInjection;

namespace CS.Mediator.Setup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services, Action<MediatorConfigurator>? configure = default)
    {
        services.AddScoped<IMediator, RequestProcessor>();
        services.AddScoped<PipelineBuilder>();

        var configurator = new MediatorConfigurator(services);
        configure?.Invoke(configurator);
        services.AddSingleton(_ => configurator.Build());
        return services;
    }
}