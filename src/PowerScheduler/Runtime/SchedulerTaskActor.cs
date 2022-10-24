using PowerScheduler.Actors;
using PowerScheduler.Entities;
using PowerScheduler.Entities.Enums;
using PowerScheduler.Runtime.Processors;
using Volo.Abp.DependencyInjection;
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

        Logger.LogInformation("start to dispatch task: {SchedulerTask}", schedulerTask);

        schedulerTask.ActualTriggerTime = Clock.Now;
        schedulerJob.LastTriggerTime = Clock.Now;

        if (schedulerJob.ProcessorInfo.IsNullOrEmpty())
        {
            Logger.LogWarning("SchedulerJob-{SchedulerJob} ProcessorInfo is empty.", schedulerJob);
            return;
        }

        var context = new ProcessorContext()
        {
            SchedulerJob = schedulerJob,
            SchedulerTask = schedulerTask,
        };

        await ExecutorRunner(context);
    }

    private async Task ExecutorRunner(ProcessorContext context)
    {
        var jobInstance = context.SchedulerTask;
        var jobInfo = context.SchedulerJob;

        try
        {
            var executeResult = await RunProcessor(context);

            jobInstance.FinishedTime = DateTime.Now;
            jobInstance.Result = executeResult.Message;
            jobInstance.TaskRunStatus = executeResult.Success ? TaskRunStatus.Succeed : TaskRunStatus.Failed;

            if (!executeResult.Success)
            {
                Logger.LogWarning("execute result is unsuccess: {Result}", executeResult.Message);
            }
        }
        catch (Exception ex)
        {
            if (jobInstance.TryCount >= jobInfo.MaxTryCount)
            {
                Logger.LogError(ex, "executor runner retry {TryCount} greater than max trycount {MaxTryCount} failed.",
                    jobInstance.TryCount, jobInfo.MaxTryCount);
                return;
            }

            jobInstance.TryCount++;
            jobInstance.TaskRunStatus = TaskRunStatus.Failed;
            jobInstance.Result = $"executor runner failed, retry: {jobInstance.TryCount}, error: {ex.Message}";
            Logger.LogWarning(ex, "executor runner failed, retry: {TryCount}", jobInstance.TryCount);
        }
    }

    private async Task<ProcessorResult> RunProcessor(ProcessorContext context)
    {
        var processor = GetProcessor(context);
        if (processor != null)
            return await processor.ExecuteAsync(context);

        return ProcessorResult.ErrorMessage($"processor type {context.SchedulerJob.ProcessorInfo} not register, Processor must implementation {typeof(IProcessor).AssemblyQualifiedName}");
    }

    private IProcessor GetProcessor(ProcessorContext context)
    {
        var instanceType = Type.GetType(context.SchedulerJob.ProcessorInfo);

        var processor = (IProcessor)ServiceProvider.GetRequiredService(instanceType);

        if (processor is ProcessorBase processorBase && processorBase.LazyServiceProvider == null)
        {
            processorBase.LazyServiceProvider = ServiceProvider.GetRequiredService<IAbpLazyServiceProvider>();
        }

        return processor;
    }
}
