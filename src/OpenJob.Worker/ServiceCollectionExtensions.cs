using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenJob.Background;
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
        services.AddHostedService<WorkerHost>();
        services.AddSingleton<WorkerHealthReporter>();
        services.AddSingleton<SingalRClient>();

        services.Scan(scan => scan
            .FromAssemblyOf<IProcessor>()
                .AddClasses(classes => classes.AssignableTo<IProcessor>())
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
            .FromAssemblyOf<IProcessorFactory>()
                .AddClasses(classes => classes.AssignableTo<IProcessorFactory>())
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());

        return services;
    }
}
