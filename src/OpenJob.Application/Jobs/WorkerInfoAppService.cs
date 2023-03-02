using OpenJob.Workers;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace OpenJob.Jobs;

public class WorkerInfoAppService :
    CrudAppService<
        WorkerInfo,
        WorkerInfoDto,
        Guid,
        GetAndFilterListDto,
        CreateUpdateWorkerDto>,
    IWorkerInfoAppService
{
    public WorkerInfoAppService(IRepository<WorkerInfo, Guid> workerRepository)
        : base(workerRepository)
    {

    }
}
