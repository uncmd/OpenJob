using OpenJob.Entities;
using OpenJob.Services.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace OpenJob.Services;

public class SchedulerTaskAppService :
    CrudAppService<
        TaskInfo,
        SchedulerTaskDto,
        Guid,
        GetAndFilterListDto,
        CreateUpdateTaskDto>
{
    public SchedulerTaskAppService(IRepository<TaskInfo, Guid> taskRepository)
        : base(taskRepository)
    {

    }
}
