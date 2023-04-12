using Orleans.Concurrency;

namespace OpenJob.Server.Actors;

public interface ISchedulerReminder : IGrainWithIntegerKey
{
    /// <summary>
    /// 激活Actor并注册Reminder
    /// </summary>
    /// <returns></returns>
    Task Active();

    /// <summary>
    /// 设置版本信息
    /// </summary>
    /// <param name="orleans"></param>
    /// <param name="host"></param>
    /// <returns></returns>
    [OneWay]
    Task SetVersion(string orleans, string host);

    /// <summary>
    /// 获取版本信息
    /// </summary>
    /// <returns></returns>
    Task<Immutable<Dictionary<string, string>>> GetExtendedProperties();
}
