namespace PowerScheduler.Runtime;

public interface ISchedulerJobActor : IGrainWithGuidKey
{
    Task Schedule(Guid appId);
}
