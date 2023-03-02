using Volo.Abp.Application.Services;

namespace OpenJob.Jobs;

public interface ITaskInfoAppService :
    ICrudAppService<
        TaskInfoDto,
        Guid,
        GetAndFilterListDto,
        CreateUpdateTaskDto>
{
}
