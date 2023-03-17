using Volo.Abp.Application.Services;

namespace OpenJob.Workers;

public interface IWorkerInfoAppService :
    ICrudAppService<
        WorkerInfoDto,
        Guid,
        GetAndFilterListDto,
        CreateUpdateWorkerDto>
{
}
