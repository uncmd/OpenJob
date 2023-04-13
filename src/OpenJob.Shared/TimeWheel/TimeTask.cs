namespace OpenJob.TimeWheel;

public class TimeTask
{
    internal volatile TimeSlot TimeSlot;

    public long TimeoutMs { get; internal set; }

    public bool PeriodTask { get; }

    public TimeSpan Period { get; }

    internal ActionOrAsyncFunc ActionTask { get; }

    public TimeTaskStatus TaskStatus { get; internal set; } = TimeTaskStatus.Waiting;

    public bool IsWaiting => TaskStatus == TimeTaskStatus.Waiting;

    private readonly object _lock = new object();

    public TimeTask(ActionOrAsyncFunc task, long timeoutMs, bool periodTask)
    {
        ActionTask = task;
        TimeoutMs = timeoutMs;
        PeriodTask = periodTask;

        if (periodTask)
        {
            var periodMs = timeoutMs - DateTime.Now.GetTimestamp();
            Period = TimeSpan.FromMilliseconds(periodMs);
        }
    }

    /// <summary>
    /// 执行任务
    /// </summary>
    internal async Task RunAsync()
    {
        if (!IsWaiting)
        {
            return;
        }

        lock (_lock)
        {
            if (IsWaiting)
            {
                TaskStatus = TimeTaskStatus.Running;
                Remove();
            }
        }

        if (TaskStatus == TimeTaskStatus.Running)
        {
            try
            {
                await ActionTask.Invoke();
                TaskStatus = TimeTaskStatus.Success;
            }
            catch
            {
                // 由DelayTask内部处理异常，这里不处理
                TaskStatus = TimeTaskStatus.Fail;
            }
        }
    }

    public bool Cancel()
    {
        if (!IsWaiting)
        {
            return false;
        }

        lock (_lock)
        {
            if (IsWaiting)
            {
                TaskStatus = TimeTaskStatus.Cancel;
                Remove();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 移除任务
    /// </summary>
    internal void Remove()
    {
        while (TimeSlot != null && !TimeSlot.RemoveTask(this))
        {
            // 如果task被另一个线程移动到了其它bucket中，就会移除失败，需要重试
        }

        TimeSlot = null;
    }
}
