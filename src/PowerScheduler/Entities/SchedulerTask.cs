using PowerScheduler.Entities.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace PowerScheduler.Entities;

public class SchedulerTask : AuditedAggregateRoot<Guid>
{
    public Guid JobId { get; private set; }

    public string TaskArgs { get; set; }

    /// <summary>
    /// 任务状态
    /// </summary>
    public TaskRunStatus TaskRunStatus { get; set; }

    /// <summary>
    /// 预计触发时间
    /// </summary>
    public DateTimeOffset ExpectedTriggerTime { get; set; }

    /// <summary>
    /// 实际触发时间
    /// </summary>
    public DateTimeOffset ActualTriggerTime { get; set; } = DateTimeOffset.MinValue;

    public DateTimeOffset FinishedTime { get; set; } = DateTimeOffset.MinValue;

    public string WorkerHost { get; set; }

    /// <summary>
    /// 执行结果
    /// </summary>
    public string Result { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int TryCount { get; set; }

    public SchedulerTask(Guid jobId)
    {
        JobId = jobId;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, Status = {TaskRunStatus}";
    }
}
