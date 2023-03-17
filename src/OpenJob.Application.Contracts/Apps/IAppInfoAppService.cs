using Volo.Abp.Application.Services;

namespace OpenJob.Apps;

public interface IAppInfoAppService :
    ICrudAppService<
        AppInfoDto,
        Guid,
        GetAndFilterListDto,
        AppInfoDto>
{
}
