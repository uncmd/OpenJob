using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace OpenJob.Apps;

public class AppInfoDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    protected IGuidGenerator GuidGenerator { get; }
    protected ICurrentTenant CurrentTenant { get; }
    protected IRepository<AppInfo, Guid> AppRepository { get; }

    public AppInfoDataSeedContributor(
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        IRepository<AppInfo, Guid> appRepository)
    {
        GuidGenerator = guidGenerator;
        CurrentTenant = currentTenant;
        AppRepository = appRepository;
    }

    public Task SeedAsync(DataSeedContext context)
    {
        return SeedAsync(context.TenantId);
    }

    [UnitOfWork]
    protected async Task SeedAsync(Guid? tenantId = null)
    {
        using (CurrentTenant.Change(tenantId))
        {
            if (await AppRepository.GetCountAsync() <= 0)
            {
                var defaultApp = new AppInfo(GuidGenerator.Create(), "Default")
                {
                    Description = "默认应用"
                };
                await AppRepository.InsertAsync(defaultApp, autoSave: true);
            }
        }
    }
}
