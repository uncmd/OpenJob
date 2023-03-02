namespace OpenJob.Server.Actors;

public interface ISchedulerJobActor : IGrainWithGuidKey
{
    Task Schedule(Guid appId);
}
