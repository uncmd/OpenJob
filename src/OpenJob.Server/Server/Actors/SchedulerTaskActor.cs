﻿using Microsoft.Extensions.Logging;
using OpenJob.Tasks;

namespace OpenJob.Server.Actors;

public class SchedulerTaskActor : ActorBase, ISchedulerTaskActor
{
    private readonly DispatchService _dispatchService;

    public SchedulerTaskActor(DispatchService dispatchService)
    {
        _dispatchService = dispatchService;
    }

    public async Task DispatchTask(Guid taskId, TimeSpan dueTime)
    {
        Logger.LogInformation("received dispatch request, dispatch starts in {DueTime}s, taskId: {TaskId}", dueTime.TotalSeconds, taskId);

        // 立即执行
        if (dueTime == TimeSpan.Zero)
        {
            await _dispatchService.Dispatch(taskId);
        }
        else
        {
            // 注册延时定时器，到期后执行一次
            RegisterTimer(_ => _dispatchService.Dispatch(taskId), null, dueTime, Timeout.InfiniteTimeSpan);
        }
    }
}
