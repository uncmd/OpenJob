﻿using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using PowerScheduler.Actors;
using PowerScheduler.Domain;
using PowerScheduler.Entities;
using PowerScheduler.Entities.Enums;
using System.Diagnostics;
using Volo.Abp.Domain.Repositories;

namespace PowerScheduler.Runtime;

public class PowerSchedulerActor : ActorBase, IPowerSchedulerActor, IRemindable
{
    private const string ReminderName = "PowerSchedulerReminder";
    private string versionOrleans;
    private string versionHost;

    private readonly PowerSchedulerOptions _options;
    private readonly IRepository<SchedulerJob, Guid> _jobRepository;
    private readonly IRepository<SchedulerTask, Guid> _taskRepository;
    private readonly TimingStrategyService _timingStrategyService;

    public PowerSchedulerActor(
        IOptions<PowerSchedulerOptions> options,
        IRepository<SchedulerJob, Guid> jobRepository,
        IRepository<SchedulerTask, Guid> taskRepository,
        TimingStrategyService timingStrategyService)
    {
        _options = options.Value;
        _jobRepository = jobRepository;
        _taskRepository = taskRepository;
        _timingStrategyService = timingStrategyService;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);

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

    public Task ReceiveReminder(string reminderName, TickStatus status)
    {
        Logger.LogInformation("PowerSchedulerActor ReceiveReminder {ReminderName}: {Status}", reminderName, status);

        return Task.CompletedTask;
    }

    protected virtual async Task Schedule(object state)
    {
        var stopwatch = Stopwatch.StartNew();

        var jobs = await _jobRepository.GetListAsync(p => p.IsEnabled && !p.IsAbandoned &&
            p.TimeExpression != TimeExpression.None && p.TimeExpression != TimeExpression.Api &&
            p.NextTriggerTime <= Clock.Now.AddSeconds(_options.SchedulePeriod.TotalSeconds * 1.3) &&
            p.BeginTime >= Clock.Now && p.EndTime <= Clock.Now);

        if (jobs.Any())
        {
            Logger.LogInformation("These {Count} jobs will be schedule", jobs.Count);

            try
            {
                // 批量写日志表
                List<SchedulerTask> schedulerTasks = new List<SchedulerTask>();
                foreach (var job in jobs)
                {
                    var taskId = GuidGenerator.Create();
                    schedulerTasks.Add(new SchedulerTask(taskId, job.Id, job.NextTriggerTime));
                }

                await _taskRepository.InsertManyAsync(schedulerTasks, true);

                // 激活Actor等待调度执行
                List<Task> tasks = new List<Task>();
                foreach (var job in jobs)
                {
                    var schedulerTask = schedulerTasks.FirstOrDefault(p => p.JobId == job.Id);

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

                            continue;
                        }
                    }
                    else
                    {
                        dueTime = targetTriggerTime - Clock.Now;
                    }

                    var taskActor = ActorClient.GetGrain<ISchedulerTaskActor>(schedulerTask.Id);
                    var task = taskActor.DispatchTask(schedulerTask.Id, dueTime);
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);

                // 计算下一次调度时间
                foreach (var job in jobs)
                {
                    RefreshJob(job);
                }

                await _jobRepository.UpdateManyAsync(jobs, true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "dispatch job failed");
            }
        }
        else
        {
            Logger.LogInformation("current has no job to schedule");
        }

        stopwatch.Stop();
        Logger.LogInformation("current schedule finish({Cost}ms)", stopwatch.ElapsedMilliseconds);

        if (stopwatch.Elapsed > _options.SchedulePeriod)
        {
            Logger.LogWarning("The database query is using too much time({Cost}ms), please check if the database load is too high!", stopwatch.ElapsedMilliseconds);
        }
    }

    protected virtual void RefreshJob(SchedulerJob schedulerJob)
    {
        try
        {
            // 计算任务下一次调度时间
            var nextTriggerTime = _timingStrategyService.CalculateNextTriggerTime(schedulerJob);

            if (nextTriggerTime.HasValue)
            {
                schedulerJob.NextTriggerTime = nextTriggerTime.Value;
            }
            else
            {
                if (schedulerJob.NeedNextTriggerTime)
                {
                    Logger.LogWarning("won't be scheduled anymore, system will set the status to Abandoned: {SchedulerJob}", schedulerJob);

                    schedulerJob.IsAbandoned = true;
                }

                if (schedulerJob.TimeExpression == TimeExpression.Delayed)
                {
                    schedulerJob.IsEnabled = false;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "refresh job failed, system will set job to Abandoned: {SchedulerJob}", schedulerJob);
            schedulerJob.IsAbandoned = false;
        }
    }
}
