using Orleans.Concurrency;

namespace OpenJob.Server.Actors;

public interface ISchedulerJobActor : IGrainWithGuidKey
{
    [OneWay]
    Task ScheduleJob(Guid appId, string appName);
}
