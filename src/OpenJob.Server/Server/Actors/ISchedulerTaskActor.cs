namespace OpenJob.Server.Actors;

public interface ISchedulerTaskActor : IGrainWithGuidKey
{
    Task DispatchTask(Guid taskId, TimeSpan dueTime);
}
