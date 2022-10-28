using PowerScheduler.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace PowerScheduler.Entities;

public class SchedulerTask : AuditedAggregateRoot<Guid>
{
    public Guid AppId { get; set; }

    public Guid JobId { get; private set; }

    public string JobArgs { get; set; }

    public string TaskArgs { get; set; }

    /// <summary>
    /// 任务状态
    /// </summary>
    public TaskRunStatus TaskRunStatus { get; set; }

    /// <summary>
    /// 预计触发时间
    /// </summary>
    public DateTime ExpectedTriggerTime { get; set; }

    /// <summary>
    /// 实际触发时间
    /// </summary>
    public DateTime ActualTriggerTime { get; set; } = DateTime.MinValue;

    public DateTime FinishedTime { get; set; } = DateTime.MinValue;

    public string WorkerHost { get; set; }

    /// <summary>
    /// 执行结果
    /// </summary>
    public string Result { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int TryCount { get; set; }

    protected SchedulerTask() { }

    public SchedulerTask(Guid jobId, DateTime expectedTriggerTime)
    {
        JobId = jobId;
        ExpectedTriggerTime = expectedTriggerTime;
        TaskRunStatus = TaskRunStatus.WaitingDispatch;
    }

    public SchedulerTask(Guid id, Guid jobId, DateTime expectedTriggerTime)
        : base(id)
    {
        JobId = jobId;
        ExpectedTriggerTime = expectedTriggerTime;
        TaskRunStatus = TaskRunStatus.WaitingDispatch;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, Status = {TaskRunStatus}";
    }
}
