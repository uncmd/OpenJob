using PowerScheduler.Entities;

namespace PowerScheduler.Runtime.Processors;

/// <summary>
/// 处理器上下文
/// </summary>
public class ProcessorContext
{
    public SchedulerJob SchedulerJob { get; set; }

    public SchedulerTask SchedulerTask { get; set; }

    /// <summary>
    /// 获取参数，优先获取实例参数，不存在则获取任务参数
    /// </summary>
    /// <returns></returns>
    public virtual string GetArgs()
    {
        if (!string.IsNullOrWhiteSpace(SchedulerTask.TaskArgs))
            return SchedulerTask.TaskArgs;
        return SchedulerJob.JobArgs;
    }

    public override string ToString()
    {
        return $"SchedulerJob: {SchedulerJob}, " +
               $"SchedulerTask: {SchedulerTask}";
    }
}
