namespace PowerScheduler.Processors;

/// <summary>
/// 处理器上下文
/// </summary>
public class ProcessorContext
{
    public Guid JobId { get; set; }

    public Guid TaskId { get; set; }

    public string JobArgs { get; set; }

    public string TaskArgs { get; set; }

    public int MaxTryCount { get; set; }

    public int TryCount { get; set; }

    /// <summary>
    /// 获取参数，优先获取实例参数，不存在则获取任务参数
    /// </summary>
    /// <returns></returns>
    public virtual string GetArgs()
    {
        if (!TaskArgs.IsNullOrWhiteSpace())
            return TaskArgs;
        return JobArgs;
    }

    public override string ToString()
    {
        return $"JobId: {JobId}, TaskId: {TaskId}";
    }
}
