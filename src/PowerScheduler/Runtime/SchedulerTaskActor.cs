using PowerScheduler.Actors;
using PowerScheduler.Entities;
using PowerScheduler.Enums;
using Volo.Abp.Domain.Repositories;

namespace PowerScheduler.Runtime;

public class SchedulerTaskActor : ActorBase, ISchedulerTaskActor
{
    private readonly IRepository<SchedulerJob, Guid> _jobRepository;
    private readonly IRepository<SchedulerTask, Guid> _taskRepository;

    public SchedulerTaskActor(
        IRepository<SchedulerJob, Guid> jobRepository,
        IRepository<SchedulerTask, Guid> taskRepository)
    {
        _jobRepository = jobRepository;
        _taskRepository = taskRepository;
    }

    public Task DispatchTask(Guid taskId, TimeSpan dueTime)
    {
        Logger.LogInformation("{DueTime}s will dispatch, taskId: {TaskId}", dueTime.TotalSeconds, taskId);

        RegisterTimer(DispatchCore, taskId, dueTime, Timeout.InfiniteTimeSpan);

        return Task.CompletedTask;
    }

    private async Task DispatchCore(object state)
    {
        var taskId = (Guid)state;
        var schedulerTask = await _taskRepository.GetAsync(taskId);

        if (schedulerTask.TaskRunStatus == TaskRunStatus.Canceled)
        {
            Logger.LogInformation("cancel dispatch due to task has been canceled: {SchedulerTask}", schedulerTask);
            return;
        }

        if (schedulerTask.TaskRunStatus != TaskRunStatus.WaitingDispatch)
        {
            Logger.LogInformation("cancel dispatch due to task has been dispatched: {SchedulerTask}", schedulerTask);
            return;
        }

        var schedulerJob = await _jobRepository.FindAsync(schedulerTask.JobId);
        if (schedulerJob == null)
        {
            Logger.LogWarning("cancel dispatch due to job has been deleted: {SchedulerTask}", schedulerTask);
            return;
        }

        if (schedulerJob.ProcessorInfo.IsNullOrEmpty())
        {
            Logger.LogWarning("SchedulerJob-{SchedulerJob} ProcessorInfo is empty.", schedulerJob);
            return;
        }

        Logger.LogInformation("start to dispatch task: {SchedulerTask}", schedulerTask);

        schedulerTask.ActualTriggerTime = Clock.Now;
        schedulerJob.LastTriggerTime = Clock.Now;
    }
}
