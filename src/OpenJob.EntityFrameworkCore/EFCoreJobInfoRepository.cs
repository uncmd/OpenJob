using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenJob.EntityFrameworkCore;
using OpenJob.Enums;
using OpenJob.Jobs;
using OpenJob.TimingStrategys;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Timing;

namespace OpenJob;

public class EFCoreJobInfoRepository :
    EfCoreRepository<OpenJobDbContext, JobInfo, Guid>,
    IJobInfoRepository
{
    private readonly OpenJobServerOptions _options;
    private readonly TimingStrategyService _timingStrategyService;

    protected IClock Clock => LazyServiceProvider.LazyGetRequiredService<IClock>();

    public EFCoreJobInfoRepository(
        IDbContextProvider<OpenJobDbContext> dbContextProvider,
        IOptions<OpenJobServerOptions> options,
        TimingStrategyService timingStrategyService)
        : base(dbContextProvider)
    {
        _options = options.Value;
        _timingStrategyService = timingStrategyService;
    }

    public async Task<List<JobInfo>> GetPreJobs(Guid appId)
    {
        var dbSet = await GetDbSetAsync();
        // 查询即将要执行的任务
        var startAt = Clock.Now.AddSeconds(_options.SchedulePeriod.TotalSeconds * 1.2);
        return await dbSet
            .Where(p => p.AppId == appId)
            .Where(p => p.NextTriggerTime <= startAt)
            .Where(p => p.JobStatus == JobStatus.Ready || p.JobStatus == JobStatus.Running || p.JobStatus == JobStatus.ErrorToReady)
            .Where(p => p.BeginTime == null || p.BeginTime >= Clock.Now)
            .Where(p => p.EndTime == null || p.EndTime <= Clock.Now)
            .ToListAsync();
    }

    public async Task RefreshNextTriggerTime(List<JobInfo> jobs)
    {
        foreach (var job in jobs)
        {
            _timingStrategyService.CalculateNextTriggerTime(job);
        }

        await UpdateManyAsync(jobs, true);
    }
}
