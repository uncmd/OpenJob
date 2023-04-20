using OpenJob.Tasks;
using OpenJob.TimeWheel;

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
        // 立即执行
        if (dueTime == TimeSpan.Zero)
        {
            await _dispatchService.Dispatch(taskId);
        }
        else
        {
            // 到期后执行一次
            TimingWheelTimer.Instance.Schedule(dueTime, () => _dispatchService.Dispatch(taskId));
        }
    }

    public async Task StopTask(Guid taskId)
    {
        await _dispatchService.StopTask(taskId);
    }
}
