using Microsoft.Extensions.DependencyInjection;
using OpenJob.Background;
using OpenJob.Core;
using OpenJob.Hosting;
using OpenJob.Processors;

namespace OpenJob;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenJobWorker(
        this IServiceCollection services,
        Action<OpenJobWorkerOptions> configurator = null)
    {
        services.Configure(configurator ?? (x => { }));
        services.AddTransient<ISystemMetricsCollector, SystemMetricsCollector>();
        services.AddTransient<IWorkerClient, WorkerClient>();
        services.AddHostedService<WorkerHost>();
        services.AddSingleton<WorkerHealthReporter>();
        services.AddSingleton<ServerClientProxy>();
        services.AddSingleton<IProcessorLoader, OpenJobProcessorLoader>();

        services.Scan(scan => scan
            .FromApplicationDependencies()
                .AddClasses(classes => classes.AssignableTo<IProcessor>())
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
                .AddClasses(classes => classes.AssignableTo<IProcessorFactory>())
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

        return services;
    }
}
