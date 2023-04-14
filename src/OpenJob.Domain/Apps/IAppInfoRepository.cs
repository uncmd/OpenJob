using Volo.Abp.Domain.Repositories;

namespace OpenJob.Apps;

public interface IAppInfoRepository : IBasicRepository<AppInfo, Guid>
{
    Task<long> GetCountAsync(
        string filter = null,
        string name = null,
        string description = null,
        bool? isEnabled = null,
        DateTime? maxCreationTime = null,
        DateTime? minCreationTime = null,
        DateTime? maxModifitionTime = null,
        DateTime? minModifitionTime = null,
        CancellationToken cancellationToken = default
    );

    Task<List<AppInfo>> GetListAsync(
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
        CancellationToken cancellationToken = default
    );

    Task<AppInfo> GetAsync(string name);
}
