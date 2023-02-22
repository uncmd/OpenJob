using PowerScheduler.Actors;
using PowerScheduler.Domain;

namespace PowerScheduler.Runtime.Actors;

public class SchedulerTaskActor : ActorBase, ISchedulerTaskActor
{
    private readonly DispatchService _dispatchService;

    public SchedulerTaskActor(DispatchService dispatchService)
    {
        _dispatchService = dispatchService;
    }

    public Task DispatchTask(Guid taskId, TimeSpan dueTime)
    {
        Logger.LogInformation("received dispatch request, dispatch starts in {DueTime}s, taskId: {TaskId}", dueTime.TotalSeconds, taskId);

        // 注册延时定时器，到期后执行一次
        RegisterTimer(_ => _dispatchService.Dispatch(taskId), null, dueTime, Timeout.InfiniteTimeSpan);

        return Task.CompletedTask;
    }
}
