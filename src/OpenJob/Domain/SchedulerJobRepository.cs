﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenJob.Data;
using OpenJob.Entities;
using OpenJob.Enums;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Timing;

namespace OpenJob.Domain;

public interface ISchedulerJobRepository : IRepository<JobInfo, Guid>
{
    Task<List<JobInfo>> GetPreJobs(Guid appId);

    Task RefreshNextTriggerTime(List<JobInfo> jobs);
}

public class SchedulerJobRepository :
    EfCoreRepository<OpenJobDbContext, JobInfo, Guid>,
    ISchedulerJobRepository
{
    private readonly OpenJobOptions _options;
    private readonly TimingStrategyService _timingStrategyService;

    protected IClock Clock => LazyServiceProvider.LazyGetRequiredService<IClock>();

    public SchedulerJobRepository(
        IDbContextProvider<OpenJobDbContext> dbContextProvider,
        IOptions<OpenJobOptions> options,
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
        var startAt = Clock.Now.AddSeconds(_options.SchedulePeriod.TotalSeconds * 1.5);
        return await dbSet
            .Where(p => p.AppId == appId &&
                (p.JobStatus == JobStatus.Ready || p.JobStatus == JobStatus.Running || p.JobStatus == JobStatus.ErrorToReady) &&
                p.NextTriggerTime <= startAt &&
                (p.BeginTime == null || p.BeginTime >= Clock.Now) &&
                (p.EndTime == null || p.EndTime <= Clock.Now))
            .ToListAsync();
    }

    public async Task RefreshNextTriggerTime(List<JobInfo> jobs)
    {
        foreach (var job in jobs)
        {
            job.Increment(_timingStrategyService, Clock.Now);
        }

        await UpdateManyAsync(jobs, true);
    }
}