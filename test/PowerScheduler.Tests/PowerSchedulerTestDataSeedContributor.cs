using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace PowerScheduler;

public class PowerSchedulerTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public Task SeedAsync(DataSeedContext context)
    {
        /* Seed additional test data... */

        return Task.CompletedTask;
    }
}
