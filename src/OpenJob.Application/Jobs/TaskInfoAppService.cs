using OpenJob.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace OpenJob.Jobs;

public class TaskInfoAppService :
    CrudAppService<
        TaskInfo,
        TaskInfoDto,
        Guid,
        GetAndFilterListDto,
        CreateUpdateTaskDto>,
    ITaskInfoAppService
{
    public TaskInfoAppService(IRepository<TaskInfo, Guid> taskRepository)
        : base(taskRepository)
    {

    }
}
