using Volo.Abp.Application.Services;

namespace OpenJob.Jobs;

public interface IJobInfoAppService :
    ICrudAppService<
        JobInfoDto,
        Guid,
        GetAndFilterListDto,
        CreateUpdateJobDto>
{
}
