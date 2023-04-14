using Microsoft.EntityFrameworkCore;
using OpenJob.Apps;
using OpenJob.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace OpenJob;

public class EFCoreAppInfoRepository :
    EfCoreRepository<OpenJobDbContext, AppInfo, Guid>,
    IAppInfoRepository
{
    public EFCoreAppInfoRepository(IDbContextProvider<OpenJobDbContext> dbContextProvider)
    : base(dbContextProvider)
    {
    }

    public async Task<long> GetCountAsync(
        string filter = null,
        string name = null,
        string description = null,
        bool? isEnabled = null,
        DateTime? maxCreationTime = null,
        DateTime? minCreationTime = null,
        DateTime? maxModifitionTime = null,
        DateTime? minModifitionTime = null,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .WhereIf(
                !filter.IsNullOrWhiteSpace(),
                u =>
                    u.Name.Contains(filter) ||
                    u.Description.Contains(filter)
            )
            .WhereIf(!string.IsNullOrWhiteSpace(name), x => x.Name == name)
            .WhereIf(!string.IsNullOrWhiteSpace(description), x => x.Description == description)
            .WhereIf(isEnabled.HasValue, x => x.IsEnabled)
            .WhereIf(maxCreationTime != null, p => p.CreationTime <= maxCreationTime)
            .WhereIf(minCreationTime != null, p => p.CreationTime >= minCreationTime)
            .WhereIf(maxModifitionTime != null, p => p.LastModificationTime <= maxModifitionTime)
            .WhereIf(minModifitionTime != null, p => p.LastModificationTime >= minModifitionTime)
            .LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<AppInfo>> GetListAsync(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string filter = null,
        bool includeDetails = false,
        string name = null,
        string description = null,
        bool? isEnabled = null,
        DateTime? maxCreationTime = null,
        DateTime? minCreationTime = null,
        DateTime? maxModifitionTime = null,
        DateTime? minModifitionTime = null,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .WhereIf(
                !filter.IsNullOrWhiteSpace(),
                u =>
                    u.Name.Contains(filter) ||
                    u.Description.Contains(filter)
            )
            .WhereIf(!string.IsNullOrWhiteSpace(name), x => x.Name == name)
            .WhereIf(!string.IsNullOrWhiteSpace(description), x => x.Description == description)
            .WhereIf(isEnabled.HasValue, x => x.IsEnabled)
            .WhereIf(maxCreationTime != null, p => p.CreationTime <= maxCreationTime)
            .WhereIf(minCreationTime != null, p => p.CreationTime >= minCreationTime)
            .WhereIf(maxModifitionTime != null, p => p.LastModificationTime <= maxModifitionTime)
            .WhereIf(minModifitionTime != null, p => p.LastModificationTime >= minModifitionTime)
            .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(AppInfo.Name) : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<AppInfo> GetAsync(string name)
    {
        return await (await GetDbSetAsync())
            .Where(p => p.Name == name)
            .FirstOrDefaultAsync();
    }
}
