﻿using Microsoft.Extensions.Logging;
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

    public DispatchService(
        IRepository<JobInfo, Guid> jobRepository,
        IRepository<TaskInfo, Guid> taskRepository,
        WorkerClusterManager workerManager,
        SchedulerTaskManager taskManager)
    {
        _jobRepository = jobRepository;
        _taskRepository = taskRepository;
        _workerManager = workerManager;
        _taskManager = taskManager;
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
            Logger.LogWarning("[Dispatcher-{JobId}|{TaskId}] cancel dispatch due to job has been deleted!", schedulerTask.JobId, taskId);
            return;
        }

        if (schedulerTask.TaskRunStatus == TaskRunStatus.Canceled)
        {
            Logger.LogInformation("[Dispatcher-{JobName}|{TaskId}] cancel dispatch due to task has been canceled", schedulerJob.Name, taskId);
            return;
        }

        if (schedulerTask.TaskRunStatus != TaskRunStatus.WaitingDispatch)
        {
            Logger.LogInformation("[Dispatcher-{JobName}|{TaskId}] cancel dispatch due to task has been dispatched", schedulerJob.Name, taskId);
            return;
        }

        if (schedulerJob.ProcessorInfo.IsNullOrEmpty())
        {
            Logger.LogWarning("[Dispatcher-{JobName}|{TaskId}] cancel dispatch due to ProcessorInfo is empty.", schedulerJob.Name, taskId);
            return;
        }

        Logger.LogInformation("[Dispatcher-{JobName}|{TaskId}] start to dispatch task.", schedulerJob.Name, taskId);

        // TODO: 并发限制

        // 获取当前最合适的 worker 列表
        var wokers = await _workerManager.GetSuitableWorkers(schedulerJob);
        if (wokers.IsNullOrEmpty())
        {
            Logger.LogWarning("[Dispatcher-{JobName}|{TaskId}] cancel dispatch job due to no worker available for app:{AppId}", schedulerJob.Name, taskId, schedulerJob.AppId);

            await _taskManager.Update4TriggerFailed(taskId, TaskRunStatus.Failed, Clock.Now, Clock.Now, OpenJobConsts.EmptyAddress, OpenJobConsts.NoWorkerAvailable);

            await _taskManager.ProcessFinishedTask(taskId, TaskRunStatus.Failed, OpenJobConsts.NoWorkerAvailable);

            return;
        }

        var workerIpList = wokers.Select(p => p.Address).ToList();
        var taskTrackerAddress = workerIpList.FirstOrDefault();

        await PostRequest(schedulerJob, schedulerTask, workerIpList);

        Logger.LogInformation("[Dispatcher-{JobName}|{TaskId}] send schedule request to TaskTracker[address:{}] successfully.", schedulerJob.Name, taskId, taskTrackerAddress);

        await _taskManager.Update4TriggerSucceed(taskId, TaskRunStatus.Succeed, Clock.Now, taskTrackerAddress);
    }

    /// <summary>
    /// 下发任务到Worker执行
    /// </summary>
    /// <param name="job"></param>
    /// <param name="task"></param>
    /// <param name="finalWorkersIpList"></param>
    /// <returns></returns>
    private static async Task PostRequest(JobInfo job, TaskInfo task, List<string> finalWorkersIpList)
    {
        var req = new ServerScheduleJobReq
        {
            JobId = job.Id,
            TaskId = task.Id,
            AllWorkerAddress = finalWorkersIpList,
            ExecutionMode = job.ExecutionMode,
            JobArgs = job.JobArgs,
            ProcessorInfo = job.ProcessorInfo,
            ProcessorType = job.ProcessorType,
            TaskArgs = task.TaskArgs,
            TaskRetryNum = 0,
            TimeExpression = job.TimeExpression,
            TimeExpressionValue = job.TimeExpressionValue,
            TimeoutSecond = job.TimeoutSecond,
        };

        await WorkerClientFactory.DispatchJob2Worker(finalWorkersIpList.FirstOrDefault(), req);
    }
}