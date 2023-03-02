using Orleans.Concurrency;

namespace OpenJob.Server.Actors;

public interface IOpenJobActor : IGrainWithIntegerKey
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
