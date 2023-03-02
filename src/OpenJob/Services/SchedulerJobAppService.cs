using OpenJob.Entities;
using OpenJob.Services.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace OpenJob.Services;

public class SchedulerJobAppService :
    CrudAppService<
        JobInfo,
        SchedulerJobDto,
        Guid,
        GetAndFilterListDto,
        CreateUpdateJobDto>
{
    public SchedulerJobAppService(IRepository<JobInfo, Guid> jobRepository)
        : base(jobRepository)
    {

    }
}
