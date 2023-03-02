using OpenJob.Entities;
using OpenJob.Services.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace OpenJob.Services;

public class SchedulerWorkerAppService :
    CrudAppService<
        WorkerInfo,
        SchedulerWorkerDto,
        Guid,
        GetAndFilterListDto,
        CreateUpdateWorkerDto>
{
    public SchedulerWorkerAppService(IRepository<WorkerInfo, Guid> workerRepository)
        : base(workerRepository)
    {

    }
}
