using Volo.Abp.Domain.Repositories;

namespace OpenJob.Jobs;

public interface IJobInfoRepository : IRepository<JobInfo, Guid>
{
    Task<List<JobInfo>> GetPreJobs(Guid appId);

    Task RefreshNextTriggerTime(List<JobInfo> jobs);
}
