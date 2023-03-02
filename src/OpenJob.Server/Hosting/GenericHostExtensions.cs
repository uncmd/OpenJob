using Microsoft.Extensions.DependencyInjection;
using OpenJob;
using OpenJob.Server;

namespace Microsoft.Extensions.Hosting;

public static class GenericHostExtensions
{
    public static ISiloBuilder UseOpenJob(
        this ISiloBuilder builder, Action<OpenJobServerOptions> configurator = null)
    {
        builder.ConfigureServices(services => services.AddOpenJob(configurator));
        builder.AddStartupTask<SchedulerStartup>();

        return builder;
    }

    public static IServiceCollection AddOpenJob(
        this IServiceCollection services, Action<OpenJobServerOptions> configurator = null)
    {
        services.Configure(configurator ?? (x => { }));

        return services;
    }
}
