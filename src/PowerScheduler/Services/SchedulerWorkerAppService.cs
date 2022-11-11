using PowerScheduler.Entities;
using PowerScheduler.Services.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace PowerScheduler.Services;

public class SchedulerWorkerAppService :
    CrudAppService<
        SchedulerWorker,
        SchedulerWorkerDto,
        Guid,
        GetAndFilterListDto,
        CreateUpdateWorkerDto>
{
    public SchedulerWorkerAppService(IRepository<SchedulerWorker, Guid> workerRepository)
        : base(workerRepository)
    {

    }
}
