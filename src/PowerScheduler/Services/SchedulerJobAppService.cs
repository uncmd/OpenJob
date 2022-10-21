using PowerScheduler.Entities;
using PowerScheduler.Services.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace PowerScheduler.Services;

public class SchedulerJobAppService : 
    CrudAppService<
        SchedulerJob, 
        SchedulerJobDto, 
        Guid, 
        GetAndFilterListDto, 
        CreateUpdateJobDto>
{
    public SchedulerJobAppService(IRepository<SchedulerJob, Guid> jobRepository)
        : base(jobRepository)
    {
        
    }
}
