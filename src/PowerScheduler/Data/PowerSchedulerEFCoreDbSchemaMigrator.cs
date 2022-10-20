using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;

namespace PowerScheduler.Data;

public class PowerSchedulerEFCoreDbSchemaMigrator : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public PowerSchedulerEFCoreDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the PowerSchedulerDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<PowerSchedulerDbContext>()
            .Database
            .MigrateAsync();
    }
}
