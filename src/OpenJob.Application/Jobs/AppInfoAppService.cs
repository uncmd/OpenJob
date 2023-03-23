using Microsoft.AspNetCore.Authorization;
using OpenJob.Apps;
using OpenJob.Permissions;
using Volo.Abp.Application.Dtos;
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
    protected IAppInfoRepository AppInfoRepository { get; }

    public AppInfoAppService(
        IRepository<AppInfo, Guid> appRepository, IAppInfoRepository appInfoRepository)
        : base(appRepository)
    {
        AppInfoRepository = appInfoRepository;

        GetPolicyName = OpenJobPermissions.Apps.Default;
        GetListPolicyName = OpenJobPermissions.Apps.Default;
        CreatePolicyName = OpenJobPermissions.Apps.Create;
        UpdatePolicyName = OpenJobPermissions.Apps.Update;
        DeletePolicyName = OpenJobPermissions.Apps.Delete;
    }

    [Authorize(OpenJobPermissions.Apps.Default)]
    public override async Task<PagedResultDto<AppInfoDto>> GetListAsync(GetAndFilterListDto input)
    {
        var count = await AppInfoRepository.GetCountAsync(input.Filter);
        var list = await AppInfoRepository.GetListAsync(input.Sorting, input.MaxResultCount, input.SkipCount, input.Filter);

        return new PagedResultDto<AppInfoDto>(
            count,
            ObjectMapper.Map<List<AppInfo>, List<AppInfoDto>>(list)
        );
    }
}
