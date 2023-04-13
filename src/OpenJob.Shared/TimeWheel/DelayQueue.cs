namespace OpenJob.TimeWheel;

/// <summary>
/// 延时队列，线程安全，参考java DelayQueue实现
/// </summary>
/// <typeparam name="T"></typeparam>
public class DelayQueue<T> where T : class, IDelayItem, IComparable<T>
{
    private readonly object _lock = new object();

    private readonly PriorityQueue<T> priorityQueue;

    /// <summary>
    /// 当前排队等待取元素的线程
    /// </summary>
    private Thread _waitThread = null;

    /// <summary>
    /// 队列当前元素数量
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return priorityQueue.Count;
            }
        }
    }

    /// <summary>
    /// 队列是否为空
    /// </summary>
    public bool IsEmpty => Count == 0;

    public DelayQueue(int capacity = 0)
    {
        priorityQueue = new PriorityQueue<T>(capacity);
    }

    /// <summary>
    /// 将指定的元素添加到此延迟队列中
    /// </summary>
    /// <param name="item">要添加的项</param>
    /// <param name="cancelToken"></param>
    /// <returns>如果将 item 添加到集内，则为 true；否则为 false</returns>
    public bool TryAdd(T item, CancellationToken cancelToken = default)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        var delay = item.GetDelay();
        if (delay < 0)
            throw new ArgumentOutOfRangeException(nameof(item), "Delay time must be greater than or equal to 0");

        if (!Monitor.TryEnter(_lock, Timeout.InfiniteTimeSpan))
        {
            return false;
        }

        if (cancelToken.IsCancellationRequested)
        {
            Monitor.Exit(_lock);
            return false;
        }

        try
        {
            priorityQueue.Enqueue(item);

            // 如果是首项，则唤醒就绪队列的首个线程准备获取锁
            if (Peek() == item)
            {
                _waitThread = null;
                Monitor.Pulse(_lock);
            }

            return true;
        }
        finally
        {
            Monitor.Exit(_lock);
        }
    }

    /// <summary>
    /// 取出首项，但不移除
    /// </summary>
    /// <returns>取出的项</returns>
    public T Peek()
    {
        lock (_lock)
        {
            return priorityQueue.Peek();
        }
    }

    /// <summary>
    /// 取出首项，但不移除
    /// </summary>
    /// <param name="item">取出的项</param>
    /// <returns>如果取到了项则为 true; 否则为 false</returns>
    public bool TryPeek(out T item)
    {
        lock (_lock)
        {
            return priorityQueue.TryPeek(out item);
        }
    }

    /// <summary>
    /// 非阻塞获取项
    /// </summary>
    /// <param name="item">取出的项或者空值</param>
    /// <returns>如果队列为空或者首项未到期则为 false; 否则为 true 并且取出值</returns>
    public bool TryTakeNoBlocking(out T item)
    {
        lock (_lock)
        {
            if (Count == 0 || priorityQueue.Peek().GetDelay() > DateTime.Now.GetTimestamp())
            {
                item = default;
                return false;
            }
            return priorityQueue.TryDequeue(out item);
        }
    }

    /// <summary>
    /// 取出项，如果未到期，则阻塞
    /// </summary>
    /// <param name="item">取出的项</param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    public bool TryTake(out T item, CancellationToken cancelToken = default)
    {
        item = null;

        if (!Monitor.TryEnter(_lock))
        {
            return false;
        }

        if (cancelToken.IsCancellationRequested)
        {
            Monitor.Exit(_lock);
            return false;
        }

        try
        {
            while (!cancelToken.IsCancellationRequested)
            {
                // 当前没有项，阻塞等待
                if (!TryPeek(out item))
                {
                    Monitor.Wait(_lock);
                    continue;
                }

                // 如果已经到期，则出队
                var delayMs = item.GetDelay() - DateTime.Now.GetTimestamp();
                if (delayMs <= 0)
                {
                    return priorityQueue.TryDequeue(out item);
                }

                // 移除引用，便于GC清理
                item = null;

                // 如果有其它线程也在等待，则阻塞等待
                if (_waitThread != null)
                {
                    Monitor.Wait(_lock);
                    continue;
                }

                // 否则当前线程设为等待线程
                var thisThread = Thread.CurrentThread;
                _waitThread = thisThread;

                try
                {
                    // 阻塞等待，如果有更早的项加入，会提前释放
                    // 否则等待delayMs时间，即当前项到期
                    // 注意，这里不能直接返回当前项，因为当前项可能被其它线程取出，所以要进入下一个循环获取
                    Monitor.Wait(_lock, (int)delayMs);
                    continue;
                }
                finally
                {
                    // 释放出来，让其它线程也可以获取
                    if (_waitThread == thisThread)
                    {
                        _waitThread = null;
                    }
                }
            }

            return false;
        }
        finally
        {
            // 当前线程已取到项，且还有剩余项，则唤醒其它就绪的线程
            if (_waitThread == null && priorityQueue.Count > 0)
            {
                Monitor.Pulse(_lock);
            }

            Monitor.Exit(_lock);
        }
    }

    /// <summary>
    /// 清理
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            priorityQueue.Clear();
        }
    }
}
