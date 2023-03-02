using OpenJob.Apps;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace OpenJob.Jobs;

public class AppInfoAppService :
    CrudAppService<
        AppInfo,
        AppInfoDto,
        Guid,
        GetAndFilterListDto,
        AppInfoDto>,
    IAppInfoAppService
{
    public AppInfoAppService(IRepository<AppInfo, Guid> jobRepository)
        : base(jobRepository)
    {

    }
}
