using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace OpenJob.Orleans;

public class OrleansDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public const string DatabaseProviderNameKey = "_Abp_DatabaseProvider";
    public const string DatabaseProviderDefaultValue = "SqlServer";

    protected ILogger<OrleansDataSeedContributor> Logger { get; }
    protected IEnumerable<IOrleansDataSeeder> OrleansDataSeeders { get; }

    public OrleansDataSeedContributor(
        ILogger<OrleansDataSeedContributor> logger,
        IEnumerable<IOrleansDataSeeder> orleansDataSeeders)
    {
        Logger = logger;
        OrleansDataSeeders = orleansDataSeeders;
    }

    public Task SeedAsync(DataSeedContext context)
    {
        return SeedAsync(
            context?[DatabaseProviderNameKey] as string ?? DatabaseProviderDefaultValue,
            context.TenantId);
    }

    [UnitOfWork]
    protected async Task SeedAsync(string databaseProvider, Guid? tenantId = null)
    {
        Logger.LogInformation("Orleans dataseed databaseProvider: {DatabaseProvider}", databaseProvider);

        foreach (var dataSeeder in OrleansDataSeeders)
        {
            await dataSeeder.SeedAsync(databaseProvider, tenantId);
        }
    }
}
