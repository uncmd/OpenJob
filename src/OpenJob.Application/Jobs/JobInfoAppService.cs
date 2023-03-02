using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace OpenJob.Jobs;

public class JobInfoAppService :
    CrudAppService<
        JobInfo,
        JobInfoDto,
        Guid,
        GetAndFilterListDto,
        CreateUpdateJobDto>,
    IJobInfoAppService
{
    public JobInfoAppService(IRepository<JobInfo, Guid> jobRepository)
        : base(jobRepository)
    {

    }
}
