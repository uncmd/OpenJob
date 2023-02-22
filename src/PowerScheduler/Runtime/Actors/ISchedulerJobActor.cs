namespace PowerScheduler.Runtime.Actors;

public interface ISchedulerJobActor : IGrainWithGuidKey
{
    Task Schedule(Guid appId);
}
