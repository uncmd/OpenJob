using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PowerScheduler.Data;
using PowerScheduler.Entities;
using PowerScheduler.Enums;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Timing;

namespace PowerScheduler.Domain;

public interface ISchedulerJobRepository : IRepository<SchedulerJob, Guid>
{
    Task<List<SchedulerJob>> GetPreJobs();

    Task RefreshNextTriggerTime(List<SchedulerJob> jobs);
}

public class SchedulerJobRepository :
    EfCoreRepository<PowerSchedulerDbContext, SchedulerJob, Guid>,
    ISchedulerJobRepository
{
    private readonly PowerSchedulerOptions _options;
    private readonly TimingStrategyService _timingStrategyService;

    protected IClock Clock => LazyServiceProvider.LazyGetRequiredService<IClock>();

    public SchedulerJobRepository(
        IDbContextProvider<PowerSchedulerDbContext> dbContextProvider,
        IOptions<PowerSchedulerOptions> options,
        TimingStrategyService timingStrategyService)
        : base(dbContextProvider)
    {
        _options = options.Value;
        _timingStrategyService = timingStrategyService;
    }

    public async Task<List<SchedulerJob>> GetPreJobs()
    {
        var dbSet = await GetDbSetAsync();
        // 查询即将要执行的任务
        var startAt = Clock.Now.AddSeconds(_options.SchedulePeriod.TotalSeconds * 1.5);
        return await dbSet
            .Where(p => (p.JobStatus == JobStatus.Ready || p.JobStatus == JobStatus.Running || p.JobStatus == JobStatus.ErrorToReady) &&
                p.NextTriggerTime <= startAt &&
                (p.BeginTime == null || p.BeginTime >= Clock.Now) &&
                (p.EndTime == null || p.EndTime <= Clock.Now))
            .ToListAsync();
    }

    public async Task RefreshNextTriggerTime(List<SchedulerJob> jobs)
    {
        foreach (var job in jobs)
        {
            job.Increment(_timingStrategyService, Clock.Now);
        }

        await UpdateManyAsync(jobs, true);
    }
}
