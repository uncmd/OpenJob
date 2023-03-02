using OpenJob;
using OpenJob.Runtime;

namespace Microsoft.Extensions.Hosting;

public static class GenericHostExtensions
{
    public static ISiloBuilder UseOpenJob(
        this ISiloBuilder builder, Action<OpenJobOptions> configurator = null)
    {
        builder.ConfigureServices(services => services.AddOpenJob(configurator));
        builder.AddStartupTask<SchedulerStartup>();

        return builder;
    }

    public static IServiceCollection AddOpenJob(
        this IServiceCollection services, Action<OpenJobOptions> configurator = null)
    {
        services.Configure(configurator ?? (x => { }));

        return services;
    }
}
