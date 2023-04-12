namespace OpenJob.Server.Actors;

public interface ISchedulerAppActor : IGrainWithIntegerKey
{
    /// <summary>
    /// Reminder调用此方法，保证Timer活跃
    /// </summary>
    /// <returns></returns>
    Task KeepAlive();
}
