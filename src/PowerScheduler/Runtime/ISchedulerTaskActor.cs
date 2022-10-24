using Orleans;

namespace PowerScheduler.Runtime;

public interface ISchedulerTaskActor : IGrainWithGuidKey
{
    Task DispatchTask(Guid taskId, TimeSpan dueTime);
}
