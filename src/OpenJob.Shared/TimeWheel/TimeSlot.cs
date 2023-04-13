namespace OpenJob.TimeWheel;

/// <summary>
/// 时间槽
/// </summary>
internal class TimeSlot : LinkedList<TimeTask>, IDelayItem, IComparable<TimeSlot>
{
    public AtomicLong TimeoutMs { get; } = new AtomicLong(0);

    /// <summary>
    /// 总任务数
    /// </summary>
    private readonly AtomicInt _taskCount;

    private readonly object _lock = new object();

    public TimeSlot(AtomicInt taskCount)
    {
        _taskCount = taskCount;
    }

    /// <summary>
    /// 设置过期时间
    /// </summary>
    /// <param name="timeoutMs"></param>
    /// <returns></returns>
    internal bool SetExpiration(long timeoutMs)
    {
        // 第一次设置槽的时间，或是复用槽时，两者才不相等
        return TimeoutMs.GetAndSet(timeoutMs) != timeoutMs;
    }

    public void AddTask(TimeTask task)
    {
        var done = false;
        while (!done)
        {
            // 先从其它队列移除掉
            // 在lock之外操作，避免死锁
            task.Remove();

            lock (_lock)
            {
                if (task.TimeSlot == null)
                {
                    AddLast(task);
                    task.TimeSlot = this;
                    _taskCount.IncrementAndGet();
                    done = true;
                }
            }
        }
    }

    public bool RemoveTask(TimeTask task)
    {
        lock (_lock)
        {
            if (task.TimeSlot == this)
            {
                if (Remove(task))
                {
                    task.TimeSlot = null;
                    _taskCount.DecrementAndGet();
                    return true;
                }

                return false;
            }
        }

        return false;
    }

    /// <summary>
    /// 输出所有任务
    /// </summary>
    /// <param name="func"></param>
    public void Flush(Action<TimeTask> func)
    {
        lock (_lock)
        {
            while (Count > 0)
            {
                var task = First.Value;

                RemoveTask(task);
                func(task);
            }

            // 重置过期时间，标识该时间槽已出队
            TimeoutMs.Value = default;
        }
    }

    public long GetDelay()
    {
        return TimeoutMs;
    }

    public int CompareTo(TimeSlot other)
    {
        return TimeoutMs.CompareTo(other.TimeoutMs);
    }
}
