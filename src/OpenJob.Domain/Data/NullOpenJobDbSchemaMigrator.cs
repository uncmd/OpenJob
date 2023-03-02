using Volo.Abp.DependencyInjection;

namespace OpenJob.Data;

/* This is used if database provider does't define
 * IOpenJobDbSchemaMigrator implementation.
 */
public class NullOpenJobDbSchemaMigrator : IOpenJobDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
