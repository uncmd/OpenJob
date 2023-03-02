using Volo.Abp.Application.Services;

namespace OpenJob.Jobs;

public interface IWorkerInfoAppService :
    ICrudAppService<
        WorkerInfoDto,
        Guid,
        GetAndFilterListDto,
        CreateUpdateWorkerDto>
{
}
