using Orleans.Concurrency;

namespace PowerScheduler.Runtime;

public interface IPowerSchedulerActor : IGrainWithIntegerKey
{
    /// <summary>
    /// 设置版本信息
    /// </summary>
    /// <param name="orleans"></param>
    /// <param name="host"></param>
    /// <returns></returns>
    [OneWay]
    Task SetVersion(string orleans, string host);

    Task<Immutable<Dictionary<string, string>>> GetExtendedProperties();

    Task Start();

    Task Stop();
}
