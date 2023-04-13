namespace OpenJob.TimeWheel;

/// <summary>
/// 分层算法时间轮计时器
/// </summary>
public class TimingWheelTimer : ITimer
{
    /// <summary>
    /// 使用默认配置的时间轮计时器 <see cref="TimingWheelOptions"/>
    /// </summary>
    public static readonly TimingWheelTimer Instance = new TimingWheelTimer();

    /// <summary>
    /// 时间槽延时队列，和时间轮共用
    /// </summary>
    private readonly DelayQueue<TimeSlot> _delayQueue = new DelayQueue<TimeSlot>();

    /// <summary>
    /// 时间轮
    /// </summary>
    private readonly TimingWheel _timingWheel;

    /// <summary>
    /// 任务总数
    /// </summary>
    private readonly AtomicInt _taskCount = new AtomicInt(0);

    public int TaskCount => _taskCount.Value;

    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
    private volatile CancellationTokenSource _cancelTokenSource;

    /// <summary>
    /// 分层算法时间轮计时器
    /// </summary>
    /// <param name="optionAction"></param>
    public TimingWheelTimer(Action<TimingWheelOptions> optionAction = null)
    {
        var option = new TimingWheelOptions();
        optionAction?.Invoke(option);
        _timingWheel = new TimingWheel(option.TickSpan, option.SlotCount, option.StartTimestamp.GetTimestamp(), _taskCount, _delayQueue);
        Start();
    }

    public TimeTask Schedule(TimeSpan timeout, Action task)
    {
        var timeoutMs = DateTime.Now.GetTimestamp() + (long)timeout.TotalMilliseconds;
        return Schedule(new ActionOrAsyncFunc(task), timeoutMs, false);
    }

    public TimeTask Schedule(TimeSpan timeout, Func<Task> task)
    {
        var timeoutMs = DateTime.Now.GetTimestamp() + (long)timeout.TotalMilliseconds;
        return Schedule(new ActionOrAsyncFunc(task), timeoutMs, false);
    }

    public TimeTask Schedule(long timeoutMs, Action task)
    {
        return Schedule(new ActionOrAsyncFunc(task), timeoutMs, false);
    }

    public TimeTask Schedule(long timeoutMs, Func<Task> task)
    {
        return Schedule(new ActionOrAsyncFunc(task), timeoutMs, false);
    }

    public TimeTask PeriodSchedule(TimeSpan period, Action task)
    {
        var timeoutMs = DateTime.Now.GetTimestamp() + (long)period.TotalMilliseconds;
        return Schedule(new ActionOrAsyncFunc(task), timeoutMs, true);
    }

    public TimeTask PeriodSchedule(TimeSpan period, Func<Task> task)
    {
        var timeoutMs = DateTime.Now.GetTimestamp() + (long)period.TotalMilliseconds;
        return Schedule(new ActionOrAsyncFunc(task), timeoutMs, true);
    }

    private TimeTask Schedule(ActionOrAsyncFunc actionTask, long timeoutMs, bool periodTask)
    {
        ArgumentNullException.ThrowIfNull(nameof(actionTask));

        _lock.EnterReadLock();
        try
        {
            var task = new TimeTask(actionTask, timeoutMs, periodTask);
            AddTask(task);
            return task;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void Stop()
    {
        if (_cancelTokenSource != null)
        {
            _cancelTokenSource.Cancel();
            _cancelTokenSource.Dispose();
            _cancelTokenSource = null;
        }
        _delayQueue.Clear();
    }

    /// <summary>
    /// 运行
    /// </summary>
    private void Run(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                Step(token);
            }
        }
        catch (Exception e)
        {
            if (e is OperationCanceledException)
            {
                return;
            }

            throw;
        }
    }

    /// <summary>
    /// 推进时间轮
    /// </summary>
    /// <param name="token"></param>
    private void Step(CancellationToken token)
    {
        // 阻塞式获取，到期的时间槽才会出队
        if (_delayQueue.TryTake(out var slot, token))
        {
            _lock.EnterWriteLock();
            try
            {
                while (!token.IsCancellationRequested)
                {
                    // 推进时间轮
                    _timingWheel.Step(slot.TimeoutMs);

                    // 到期的任务会重新添加进时间轮，那么下一层时间轮的任务重新计算后可能会进入上层时间轮。
                    // 这样就实现了任务在时间轮中的传递，由大精度的时间轮进入小精度的时间轮。
                    slot.Flush(AddTask);

                    // Flush之后可能有新的slot入队，可能仍旧过期，因此尝试继续处理，直到没有过期项。
                    if (!_delayQueue.TryTakeNoBlocking(out slot))
                    {
                        break;
                    }
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }

    /// <summary>
    /// 添加任务
    /// </summary>
    /// <param name="timeTask">延时任务</param>
    private void AddTask(TimeTask timeTask)
    {
        // 添加失败，说明该任务已到期，需要执行了
        if (!_timingWheel.AddTask(timeTask) && timeTask.IsWaiting)
        {
            // TODO：自定义线程池?
            Task.Run(timeTask.RunAsync);

            // 周期任务重新添加到时间轮
            if (timeTask.PeriodTask)
            {
                var timeoutMs = DateTime.Now.GetTimestamp() + (long)timeTask.Period.TotalMilliseconds;
                var newTask = new TimeTask(timeTask.ActionTask, timeoutMs, true);

                AddTask(newTask);
            }
        }
    }

    /// <summary>
    /// 启动
    /// </summary>
    private void Start()
    {
        if (_cancelTokenSource != null)
        {
            return;
        }

        // 时间轮运行线程
        _cancelTokenSource = new CancellationTokenSource();
        Task.Factory.StartNew(() => Run(_cancelTokenSource.Token),
            _cancelTokenSource.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
    }
}
