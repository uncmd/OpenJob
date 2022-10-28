using PowerScheduler.Entities;
using PowerScheduler.Enums;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace PowerScheduler.Domain;

public class SchedulerTaskManager : DomainService
{
    private readonly IRepository<SchedulerTask, Guid> _taskRepository;

    public SchedulerTaskManager(IRepository<SchedulerTask, Guid> taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task Update4TriggerFailed(Guid taskId, TaskRunStatus status, DateTime actualTriggerTime, DateTime finishedTime, string workerHost, string result)
    {
        var task = await _taskRepository.GetAsync(taskId);

        task.TaskRunStatus = status;
        task.ActualTriggerTime = actualTriggerTime;
        task.FinishedTime = finishedTime;
        task.WorkerHost = workerHost;
        task.Result = result;

        await _taskRepository.UpdateAsync(task);
    }

    public async Task Update4TriggerSucceed(Guid taskId, TaskRunStatus status, DateTime actualTriggerTime, string workerHost)
    {
        var task = await _taskRepository.GetAsync(taskId);

        task.TaskRunStatus = status;
        task.ActualTriggerTime = actualTriggerTime;
        task.WorkerHost = workerHost;

        await _taskRepository.UpdateAsync(task);
    }

    /// <summary>
    /// 收尾完成的任务实例
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="status"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public async Task ProcessFinishedTask(Guid taskId, TaskRunStatus status, string result)
    {
        Logger.LogInformation("[Task-{TaskId}] process finished, final status is {TaskRunStatus}.", taskId, status);

        if (status == TaskRunStatus.Failed)
        {
            await Alert(taskId, result);
        }
    }

    /// <summary>
    /// 告警
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    private Task Alert(Guid taskId, string result)
    {
        // TODO: 告警

        return Task.CompletedTask;
    }
}
