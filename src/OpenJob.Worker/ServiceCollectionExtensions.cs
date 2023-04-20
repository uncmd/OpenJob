using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        services.AutoAddProcessor();
        services.AutoAddProcessorFactory();

        return services;
    }

    private static void AutoAddProcessor(this IServiceCollection services)
    {
        var serviceType = typeof(IProcessor);
        foreach (var assembly in AssemblyHelper.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes()
                .Where(t => serviceType.IsAssignableFrom(t) &&
                    !t.IsInterface &&
                    !t.IsAbstract &&
                    !t.IsGenericType))
            {
                services.AddTransient(type);
            }
        }
    }

    private static void AutoAddProcessorFactory(this IServiceCollection services)
    {
        var serviceType = typeof(IProcessorFactory);
        foreach (var assembly in AssemblyHelper.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes()
                .Where(t => serviceType.IsAssignableFrom(t) &&
                    !t.IsInterface &&
                    !t.IsAbstract &&
                    !t.IsGenericType))
            {
                services.AddTransient(serviceType, type);
            }
        }
    }
}
