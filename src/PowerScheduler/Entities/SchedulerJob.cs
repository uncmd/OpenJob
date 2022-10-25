using PowerScheduler.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace PowerScheduler.Entities;

public class SchedulerJob : FullAuditedAggregateRoot<Guid>
{
    #region 任务基本信息

    public string Name { get; set; }

    public string Description { get; set; }

    /// <summary>
    /// 标签,多个用逗号分隔
    /// </summary>
    public string Labels { get; set; }

    /// <summary>
    /// 任务优先级
    /// </summary>
    public JobPriority JobPriority { get; set; }

    public string JobArgs { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 如果该任务连续失败并且不会再次执行，则会出现这种情况
    /// </summary>
    public bool IsAbandoned { get; set; }

    #endregion

    #region 执行方式

    public JobType JobType { get; set; }

    public ExecutionMode ExecutionMode { get; set; }

    /// <summary>
    /// 执行器信息（执行器全称FullName）
    /// </summary>
    public string ProcessorInfo { get; set; }

    #endregion

    #region 定时参数
    /// <summary>
    /// 时间表达式类型
    /// </summary>
    public TimeExpression TimeExpression { get; set; }

    /// <summary>
    /// 时间表达式值
    /// </summary>
    public string TimeExpressionValue { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? BeginTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
    #endregion

    #region 运行时配置
    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxTryCount { get; set; }

    /// <summary>
    /// 超时时间
    /// </summary>
    public int TimeoutSecond { get; set; }

    /// <summary>
    /// 下次调度时间
    /// </summary>
    public DateTime NextTriggerTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// 最后一次调度时间
    /// </summary>
    public DateTime? LastTriggerTime { get; set; }

    /// <summary>
    /// 过期策略(忽略、立即触发补偿一次)，默认为忽略
    /// </summary>
    public MisfireStrategy MisfireStrategy { get; set; }
    #endregion

    public virtual ICollection<SchedulerTask> SchedulerTasks { get; set; }

    public bool NeedNextTriggerTime => 
        TimeExpression == TimeExpression.Cron ||
        TimeExpression == TimeExpression.FixedRate ||
        TimeExpression == TimeExpression.SecondDelay;

    public override string ToString()
    {
        return $"{base.ToString()}, Name = {Name}";
    }
}
