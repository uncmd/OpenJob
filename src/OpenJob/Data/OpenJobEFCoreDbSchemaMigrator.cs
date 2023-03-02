using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;

namespace OpenJob.Data;

public class OpenJobEFCoreDbSchemaMigrator : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public OpenJobEFCoreDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the OpenJobDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<OpenJobDbContext>()
            .Database
            .MigrateAsync();
    }
}
