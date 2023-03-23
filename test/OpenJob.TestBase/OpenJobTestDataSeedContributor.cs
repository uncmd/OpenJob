using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace OpenJob;

public class OpenJobTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public Task SeedAsync(DataSeedContext context)
    {
        /* Seed additional test data... */

        return Task.CompletedTask;
    }
}
