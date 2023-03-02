namespace OpenJob.Runtime.Actors;

public interface ISchedulerTaskActor : IGrainWithGuidKey
{
    Task DispatchTask(Guid taskId, TimeSpan dueTime);
}
