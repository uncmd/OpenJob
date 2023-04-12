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
            Increment(job, _timingStrategyService, Clock.Now);
        }

        await UpdateManyAsync(jobs, true);
    }

    /// <summary>
    /// 记录运行信息和计算下一个触发时间
    /// </summary>
    /// <param name="timingStrategyService"></param>
    /// <param name="startAt">起始时间</param>
    internal static void Increment(JobInfo jobInfo, TimingStrategyService timingStrategyService, DateTime startAt)
    {
        // 阻塞状态并没有实际执行，此时忽略次数递增和最近运行时间赋值
        if (jobInfo.JobStatus != JobStatus.Blocked)
        {
            jobInfo.LastTriggerTime = jobInfo.NextTriggerTime;
            jobInfo.NumberOfRuns++;
        }

        // 计算任务下一次调度时间
        jobInfo.NextTriggerTime = timingStrategyService.CalculateNextTriggerTime(jobInfo, startAt);

        // 检查下一次执行信息
        CheckAndFixNextOccurrence(jobInfo);
    }



    /// <summary>
    /// 检查下一次执行信息并修正 <see cref="NextTriggerTime"/> 和 <see cref="JobStatus"/>
    /// </summary>
    /// <returns></returns>
    internal static bool CheckAndFixNextOccurrence(JobInfo jobInfo)
    {
        // 检查作业执行信息
        if (jobInfo.ProcessorInfo.IsNullOrEmpty())
        {
            jobInfo.JobStatus = JobStatus.Unhandled;
            jobInfo.NextTriggerTime = null;
            return false;
        }

        // 开始时间检查
        if (jobInfo.BeginTime != null && jobInfo.NextTriggerTime != null && jobInfo.BeginTime.Value > jobInfo.NextTriggerTime.Value)
        {
            jobInfo.JobStatus = JobStatus.Backlog;
            jobInfo.NextTriggerTime = null;
            return false;
        }

        // 结束时间检查
        if (jobInfo.EndTime != null && jobInfo.NextTriggerTime != null && jobInfo.EndTime.Value < jobInfo.NextTriggerTime.Value)
        {
            jobInfo.JobStatus = JobStatus.Archived;
            jobInfo.NextTriggerTime = null;
            return false;
        }

        // 最大次数判断
        if (jobInfo.MaxNumberOfRuns > 0 && jobInfo.NumberOfRuns >= jobInfo.MaxNumberOfRuns)
        {
            jobInfo.JobStatus = JobStatus.Overrun;
            jobInfo.NextTriggerTime = null;
            return false;
        }

        // 最大错误数判断
        if (jobInfo.MaxNumberOfErrors > 0 && jobInfo.NumberOfErrors >= jobInfo.MaxNumberOfErrors)
        {
            jobInfo.JobStatus = JobStatus.Panic;
            jobInfo.NextTriggerTime = null;
            return false;
        }

        // 状态检查
        if (!jobInfo.IsNormalStatus())
        {
            return false;
        }

        // 下一次运行时间空判断
        if (jobInfo.NextTriggerTime == null)
        {
            if (jobInfo.IsNormalStatus())
                jobInfo.JobStatus = JobStatus.Unoccupied;
            return false;
        }

        return true;
    }
}
