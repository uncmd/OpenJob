using Volo.Abp.Application.Services;

namespace OpenJob.Jobs;

public interface IAppInfoAppService :
    ICrudAppService<
        AppInfoDto,
        Guid,
        GetAndFilterListDto,
        AppInfoDto>
{
}
