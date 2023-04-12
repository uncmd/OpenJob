using Orleans.Concurrency;

namespace OpenJob.Server.Actors;

public interface ISchedulerTaskActor : IGrainWithGuidKey
{
    [OneWay]
    Task DispatchTask(Guid taskId, TimeSpan dueTime);
}
