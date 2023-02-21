using Microsoft.Extensions.Options;
using Orleans.Concurrency;
using Orleans.Runtime;
using PowerScheduler.Actors;
using PowerScheduler.Domain;
using PowerScheduler.Entities;
using PowerScheduler.Enums;
using System.Diagnostics;
using Volo.Abp.Domain.Repositories;

namespace PowerScheduler.Runtime;

public class PowerSchedulerActor : ActorBase, IPowerSchedulerActor, IRemindable
{
    private string versionOrleans;
    private string versionHost;
    private IGrainReminder _grainReminder;

    private readonly PowerSchedulerOptions _options;
    private readonly ISchedulerJobRepository _jobRepository;
    private readonly IRepository<SchedulerTask, Guid> _taskRepository;

    public PowerSchedulerActor(
        IOptions<PowerSchedulerOptions> options,
        ISchedulerJobRepository jobRepository,
        IRepository<SchedulerTask, Guid> taskRepository)
    {
        _options = options.Value;
        _jobRepository = jobRepository;
        _taskRepository = taskRepository;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);

        Logger.LogInformation("PowerScheduler activate, it will register a timer period: {SchedulePeriod}", _options.SchedulePeriod);

        RegisterTimer(Schedule, null, TimeSpan.FromSeconds(5), _options.SchedulePeriod);
    }

    public Task SetVersion(string orleans, string host)
    {
        versionOrleans = orleans;
        versionHost = host;

        return Task.CompletedTask;
    }

    public Task<Immutable<Dictionary<string, string>>> GetExtendedProperties()
    {
        var results = new Dictionary<string, string>
        {
            ["HostVersion"] = versionHost,
            ["OrleansVersion"] = versionOrleans
        };

        return Task.FromResult(results.AsImmutable());
    }

    public async Task Start()
    {
        _grainReminder = await this.GetReminder(PowerSchedulerConsts.SchedulerReminderName);

        if (_grainReminder == null)
        {
            var dueTime = TimeSpan.FromSeconds(5);
            var period = TimeSpan.FromMinutes(1);

            Logger.LogInformation("Register PowerScheduler Reminder: {ReminderName}, dueTime: {DueTime}, period: {Period}", PowerSchedulerConsts.SchedulerReminderName, dueTime, period);

            _grainReminder = await this.RegisterOrUpdateReminder(PowerSchedulerConsts.SchedulerReminderName, dueTime, period);
        }
    }

    public async Task Stop()
    {
        if (_grainReminder != null)
        {
            Logger.LogInformation("Unregister PowerScheduler Reminder: {ReminderName}", _grainReminder.ReminderName);

            await this.UnregisterReminder(_grainReminder);
            _grainReminder = null;
        }
    }

    public Task ReceiveReminder(string reminderName, TickStatus status)
    {
        Logger.LogInformation("ReceiveReminder {ReminderName}: {Status}", reminderName, status);

        return Task.CompletedTask;
    }

    protected virtual async Task Schedule(object state)
    {
        var stopwatch = Stopwatch.StartNew();

        // 查询即将要执行的任务
        var jobs = await _jobRepository.GetPreJobs();
        if (!jobs.Any())
        {
            Logger.LogInformation("current has no job to schedule");
            return;
        }

        try
        {
            await ScheduleCronJob(jobs);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "dispatch job failed");
        }

        stopwatch.Stop();
        Logger.LogInformation("current schedule finish({Cost}ms)", stopwatch.ElapsedMilliseconds);

        if (stopwatch.Elapsed > _options.SchedulePeriod)
        {
            Logger.LogWarning("The database query is using too much time({Cost}ms), please check if the database load is too high!", stopwatch.ElapsedMilliseconds);
        }
    }

    protected virtual async Task ScheduleCronJob(List<SchedulerJob> jobs)
    {
        Logger.LogDebug("These jobs will be scheduled: {@Jobs}", jobs);

        // 批量写任务表
        List<SchedulerTask> schedulerTasks = new List<SchedulerTask>();
        foreach (var job in jobs)
        {
            schedulerTasks.Add(new SchedulerTask(GuidGenerator.Create(), job.Id, job.NextTriggerTime.Value));
        }

        await _taskRepository.InsertManyAsync(schedulerTasks, true);

        // 激活Actor等待调度执行
        List<Task> tasks = new List<Task>();
        foreach (var job in jobs)
        {
            var schedulerTask = schedulerTasks.FirstOrDefault(p => p.JobId == job.Id);

            var dueTime = GetJobDueTime(job);
            if (dueTime == null)
                continue;

            var taskActor = ActorClient.GetGrain<ISchedulerTaskActor>(schedulerTask.Id);
            var task = taskActor.DispatchTask(schedulerTask.Id, dueTime.Value);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        await _jobRepository.RefreshNextTriggerTime(jobs);
    }

    /// <summary>
    /// 到期时间，若小于当前时间(过期)则应用过期策略
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    private TimeSpan? GetJobDueTime(SchedulerJob job)
    {
        TimeSpan dueTime = TimeSpan.Zero;
        var targetTriggerTime = job.NextTriggerTime;
        if (targetTriggerTime < Clock.Now)
        {
            Logger.LogWarning("schedule delay, expect: {NextTriggerTime}, current: {Now}",
                targetTriggerTime, Clock.Now);

            if (job.MisfireStrategy == MisfireStrategy.Ignore &&
                job.TimeExpression != TimeExpression.Delayed)
            {
                Logger.LogInformation("MisfireStrategy is {MisfireStrategy}, continue this job, next trigger time is {NextTriggerTime}", job.MisfireStrategy, job.NextTriggerTime);

                return null;
            }
        }
        else
        {
            dueTime = targetTriggerTime.Value - Clock.Now;
        }

        return dueTime;
    }
}
