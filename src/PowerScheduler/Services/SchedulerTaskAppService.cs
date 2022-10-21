using PowerScheduler.Entities;
using PowerScheduler.Services.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace PowerScheduler.Services;

public class SchedulerTaskAppService :
    CrudAppService<
        SchedulerTask,
        SchedulerTaskDto,
        Guid,
        GetAndFilterListDto,
        CreateUpdateTaskDto>
{
    public SchedulerTaskAppService(IRepository<SchedulerTask, Guid> taskRepository)
        : base(taskRepository)
    {

    }
}
