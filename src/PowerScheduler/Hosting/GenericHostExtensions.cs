using PowerScheduler;
using PowerScheduler.Runtime;

namespace Microsoft.Extensions.Hosting;

public static class GenericHostExtensions
{
    public static ISiloBuilder UsePowerScheduler(
        this ISiloBuilder builder, Action<PowerSchedulerOptions> configurator = null)
    {
        builder.ConfigureServices(services => services.AddPowerScheduler(configurator));
        builder.AddStartupTask<SchedulerStartup>();

        return builder;
    }

    public static IServiceCollection AddPowerScheduler(
        this IServiceCollection services, Action<PowerSchedulerOptions> configurator = null)
    {
        services.Configure(configurator ?? (x => { }));

        return services;
    }
}
