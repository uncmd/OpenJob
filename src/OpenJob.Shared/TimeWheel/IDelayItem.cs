namespace OpenJob.TimeWheel;

/// <summary>
/// 延时队列任务项
/// </summary>
public interface IDelayItem
{
    /// <summary>
    /// 获取延时
    /// </summary>
    /// <returns></returns>
    long GetDelay();
}
