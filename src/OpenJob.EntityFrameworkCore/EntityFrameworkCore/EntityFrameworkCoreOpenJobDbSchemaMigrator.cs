using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenJob.Data;
using Volo.Abp.DependencyInjection;

namespace OpenJob.EntityFrameworkCore;

public class EntityFrameworkCoreOpenJobDbSchemaMigrator
    : IOpenJobDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreOpenJobDbSchemaMigrator(
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
