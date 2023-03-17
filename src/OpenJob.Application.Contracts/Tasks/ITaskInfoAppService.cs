using Volo.Abp.Application.Services;

namespace OpenJob.Tasks;

public interface ITaskInfoAppService :
    ICrudAppService<
        TaskInfoDto,
        Guid,
        GetAndFilterListDto,
        CreateUpdateTaskDto>
{
}
