namespace OpenJob.TimeWheel;

/// <summary>
/// 定时器
/// </summary>
public interface ITimer
{
    /// <summary>
    /// 任务总数
    /// </summary>
    int TaskCount { get; }

    /// <summary>
    ///  调度定时任务
    /// </summary>
    /// <param name="timeout">过期时间，相对时间</param>
    /// <param name="task">延时任务，请在内部处理异常</param>
    /// <returns></returns>
    TimeTask Schedule(TimeSpan timeout, Action task);

    /// <summary>
    /// 调度定时任务
    /// </summary>
    /// <param name="timeout">过期时间，相对时间</param>
    /// <param name="task">延时任务，请在内部处理异常</param>
    /// <returns></returns>
    TimeTask Schedule(TimeSpan timeout, Func<Task> task);

    /// <summary>
    /// 调度定时任务
    /// </summary>
    /// <param name="timeoutMs">过期时间戳，绝对时间</param>
    /// <param name="task">延时任务，请在内部处理异常</param>
    /// <returns></returns>
    TimeTask Schedule(long timeoutMs, Action task);

    /// <summary>
    /// 调度定时任务
    /// </summary>
    /// <param name="timeoutMs">过期时间戳，绝对时间</param>
    /// <param name="task">延时任务，请在内部处理异常</param>
    /// <returns></returns>
    TimeTask Schedule(long timeoutMs, Func<Task> task);

    /// <summary>
    ///  调度周期任务
    /// </summary>
    /// <param name="period">过期时间，相对时间</param>
    /// <param name="task">延时任务，请在内部处理异常</param>
    /// <returns></returns>
    TimeTask PeriodSchedule(TimeSpan period, Action task);

    /// <summary>
    /// 调度周期任务
    /// </summary>
    /// <param name="period">过期时间，相对时间</param>
    /// <param name="task">延时任务，请在内部处理异常</param>
    /// <returns></returns>
    TimeTask PeriodSchedule(TimeSpan period, Func<Task> task);

    /// <summary>
    /// 停止所有调度任务
    /// </summary>
    /// <returns></returns>
    void Stop();
}
