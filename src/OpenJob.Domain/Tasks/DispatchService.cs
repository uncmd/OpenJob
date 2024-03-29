﻿using Microsoft.Extensions.Logging;
using OpenJob.Core;
using OpenJob.Enums;
using OpenJob.Jobs;
using OpenJob.Model;
using OpenJob.Workers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace OpenJob.Tasks;

public class DispatchService : DomainService
{
    private readonly IRepository<JobInfo, Guid> _jobRepository;
    private readonly IRepository<TaskInfo, Guid> _taskRepository;
    private readonly WorkerClusterManager _workerManager;
    private readonly SchedulerTaskManager _taskManager;
    private readonly IWorkerClient _workerClient;

    public DispatchService(
        IRepository<JobInfo, Guid> jobRepository,
        IRepository<TaskInfo, Guid> taskRepository,
        WorkerClusterManager workerManager,
        SchedulerTaskManager taskManager,
        IWorkerClient workerClient)
    {
        _jobRepository = jobRepository;
        _taskRepository = taskRepository;
        _workerManager = workerManager;
        _taskManager = taskManager;
        _workerClient = workerClient;
    }

    /// <summary>
    /// 将任务从Server派发到Worker
    /// </summary>
    /// <remarks>只会派发当前状态为等待派发的任务</remarks>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public async Task Dispatch(Guid taskId)
    {
        var schedulerTask = await _taskRepository.GetAsync(taskId);
        if (schedulerTask == null)
        {
            Logger.LogWarning("[Dispatcher-{TaskId}] cancel dispatch due to task has been deleted!", taskId);
            return;
        }

        var schedulerJob = await _jobRepository.FindAsync(schedulerTask.JobId);
        if (schedulerJob == null)
        {
            Logger.LogWarning("Dispatcher-{JobId}|{TaskId} cancel dispatch due to job has been deleted!",
                schedulerTask.JobId, taskId);
            return;
        }

        using (Logger.BeginScope("Dispatcher-{JobName}-{JobId}|{TaskId}", 
            schedulerJob.Name, schedulerTask.JobId, taskId))
        {
            if (schedulerTask.TaskRunStatus == TaskRunStatus.Canceled)
            {
                Logger.LogInformation("cancel dispatch due to task has been canceled.");
                return;
            }

            if (schedulerTask.TaskRunStatus != TaskRunStatus.WaitingDispatch)
            {
                Logger.LogInformation("cancel dispatch due to task has been dispatched.");
                return;
            }

            if (schedulerJob.ProcessorInfo.IsNullOrEmpty())
            {
                Logger.LogWarning("cancel dispatch due to ProcessorInfo is empty.");
                return;
            }

            Logger.LogDebug("start to dispatch task.");

            // TODO: 并发限制

            // 获取当前最合适的 worker 列表
            var wokers = await _workerManager.GetSuitableWorkers(schedulerJob);
            if (wokers.IsNullOrEmpty())
            {
                Logger.LogWarning("cancel dispatch job due to no worker available.");

                await _taskManager.Update4TriggerFailed(taskId, TaskRunStatus.Failed, Clock.Now, Clock.Now, OpenJobConsts.EmptyAddress, OpenJobConsts.NoWorkerAvailable);

                await _taskManager.ProcessFinishedTask(taskId, TaskRunStatus.Failed, OpenJobConsts.NoWorkerAvailable);

                return;
            }

            var connectionIdAddress = wokers.ToDictionary(p => p.ConnectionId, p => p.Address);
            await PostRequest(schedulerJob, schedulerTask, connectionIdAddress);
            var trackerAddress = wokers.Select(p => p.Address).FirstOrDefault();

            await _taskManager.Update4TriggerSucceed(taskId, TaskRunStatus.Succeed, Clock.Now, trackerAddress);
        }
    }

    /// <summary>
    /// 终止任务
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public async Task StopTask(Guid taskId)
    {
        await _workerClient.StopJob(taskId);
    }

    /// <summary>
    /// 下发任务到Worker执行
    /// </summary>
    /// <param name="job"></param>
    /// <param name="task"></param>
    /// <param name="connectionIdAddress"></param>
    /// <returns></returns>
    private async Task PostRequest(JobInfo job, TaskInfo task, Dictionary<string, string> connectionIdAddress)
    {
        var req = new ServerScheduleJobReq
        {
            JobId = job.Id,
            JobName = job.Name,
            TaskId = task.Id,
            ConnectionIdAddress = connectionIdAddress,
            ExecutionMode = job.ExecutionMode,
            JobArgs = job.JobArgs,
            ProcessorInfo = job.ProcessorInfo,
            ProcessorType = job.ProcessorType,
            TaskArgs = task.TaskArgs,
            TaskRetryNum = 0,
            TimeExpression = job.TimeExpression,
            TimeExpressionValue = job.TimeExpressionValue,
            TimeoutSecond = job.TimeoutSecond,
            DispatchStrategy = job.DispatchStrategy,
        };

        await _workerClient.RunJob(req);
    }
}
