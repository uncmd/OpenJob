using PowerScheduler.Domain;
using PowerScheduler.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace PowerScheduler.Entities;

public sealed class SchedulerJob : FullAuditedAggregateRoot<Guid>
{
    #region 任务基本信息

    public Guid AppId { get; private set; }

    public string Name { get; private set; }

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
    /// 作业状态
    /// </summary>
    public JobStatus JobStatus { get; set; }

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
    public DateTime? NextTriggerTime { get; set; }

    /// <summary>
    /// 最后一次调度时间
    /// </summary>
    public DateTime? LastTriggerTime { get; set; }

    /// <summary>
    /// 过期策略(忽略、立即触发补偿一次)，默认为忽略
    /// </summary>
    public MisfireStrategy MisfireStrategy { get; set; }

    /// <summary>
    /// 最低CPU核心数量，0代表不限
    /// </summary>
    public double MinCpuCores { get; set; }

    /// <summary>
    /// 最低内存空间，单位 GB，0代表不限
    /// </summary>
    public double MinMemory { get; set; }

    /// <summary>
    /// 最低磁盘空间，单位 GB，0代表不限
    /// </summary>
    public double MinDisk { get; set; }

    /// <summary>
    /// 触发次数
    /// </summary>
    public long NumberOfRuns { get; set; }

    /// <summary>
    /// 最大触发次数
    /// </summary>
    /// <remarks>
    /// <para>0：不限制</para>
    /// <para>n：N 次</para>
    /// </remarks>
    public long MaxNumberOfRuns { get; set; }

    /// <summary>
    /// 出错次数
    /// </summary>
    public long NumberOfErrors { get; set; }

    /// <summary>
    /// 最大出错次数
    /// </summary>
    /// <remarks>
    /// <para>0：不限制</para>
    /// <para>n：N 次</para>
    /// </remarks>
    public long MaxNumberOfErrors { get; set; }
    #endregion

    private SchedulerJob() { }

    public SchedulerJob(Guid id, Guid appId, string name) 
        : base(id)
    {
        AppId = appId;
        Name = name;

        JobPriority = JobPriority.Normal;
        JobStatus = JobStatus.NotStart;
        JobType = JobType.CSharp;
        ExecutionMode = ExecutionMode.Standalone;

    }

    /// <summary>
    /// 下一次可执行检查
    /// </summary>
    /// <param name="startAt">起始时间</param>
    /// <returns><see cref="bool"/></returns>
    internal bool NextShouldRun(DateTime startAt)
    {
        return IsNormalStatus()
            && NextTriggerTime.HasValue
            && NextTriggerTime.Value <= startAt
            && LastTriggerTime != NextTriggerTime
            && (BeginTime == null || BeginTime >= startAt)
            && (EndTime == null || EndTime <= startAt);
    }

    /// <summary>
    /// 记录运行信息和计算下一个触发时间
    /// </summary>
    /// <param name="timingStrategyService"></param>
    /// <param name="startAt">起始时间</param>
    internal void Increment(TimingStrategyService timingStrategyService, DateTime startAt)
    {
        // 阻塞状态并没有实际执行，此时忽略次数递增和最近运行时间赋值
        if (JobStatus != JobStatus.Blocked)
        {
            LastTriggerTime = NextTriggerTime;
            NumberOfRuns++;
        }

        // 计算任务下一次调度时间
        NextTriggerTime = timingStrategyService.CalculateNextTriggerTime(this, startAt);

        // 检查下一次执行信息
        CheckAndFixNextOccurrence();
    }

    /// <summary>
    /// 记录错误信息，包含错误次数和运行状态
    /// </summary>
    /// <param name="startAt">起始时间</param>
    internal void IncrementErrors(DateTime startAt)
    {
        NumberOfErrors++;

        // 检查下一次执行信息
        if (CheckAndFixNextOccurrence())
            JobStatus = JobStatus.ErrorToReady;
    }

    /// <summary>
    /// 检查下一次执行信息并修正 <see cref="NextTriggerTime"/> 和 <see cref="JobStatus"/>
    /// </summary>
    /// <returns></returns>
    internal bool CheckAndFixNextOccurrence()
    {
        // 检查作业执行信息
        if (ProcessorInfo.IsNullOrEmpty())
        {
            JobStatus = JobStatus.Unhandled;
            NextTriggerTime = null;
            return false;
        }

        // 开始时间检查
        if (BeginTime != null && NextTriggerTime != null && BeginTime.Value > NextTriggerTime.Value)
        {
            JobStatus = JobStatus.Backlog;
            NextTriggerTime = null;
            return false;
        }

        // 结束时间检查
        if (EndTime != null && NextTriggerTime != null && EndTime.Value < NextTriggerTime.Value)
        {
            JobStatus = JobStatus.Archived;
            NextTriggerTime = null;
            return false;
        }

        // 最大次数判断
        if (MaxNumberOfRuns > 0 && NumberOfRuns >= MaxNumberOfRuns)
        {
            JobStatus = JobStatus.Overrun;
            NextTriggerTime = null;
            return false;
        }

        // 最大错误数判断
        if (MaxNumberOfErrors > 0 && NumberOfErrors >= MaxNumberOfErrors)
        {
            JobStatus = JobStatus.Panic;
            NextTriggerTime = null;
            return false;
        }

        // 状态检查
        if (!IsNormalStatus())
        {
            return false;
        }

        // 下一次运行时间空判断
        if (NextTriggerTime == null)
        {
            if (IsNormalStatus())
                JobStatus = JobStatus.Unoccupied;
            return false;
        }

        return true;
    }

    /// <summary>
    /// 是否是正常触发器状态
    /// </summary>
    /// <returns><see cref="bool"/></returns>
    internal bool IsNormalStatus()
    {
        var isNormalStatus = JobStatus != JobStatus.Backlog
            && JobStatus != JobStatus.Pause
            && JobStatus != JobStatus.Archived
            && JobStatus != JobStatus.Panic
            && JobStatus != JobStatus.Overrun
            && JobStatus != JobStatus.Unoccupied
            && JobStatus != JobStatus.NotStart
            && JobStatus != JobStatus.Unknown
            && JobStatus != JobStatus.Unhandled;

        // 如果不是正常触发器状态，NextRunTime 强制设置为 null
        if (!isNormalStatus) 
            NextTriggerTime = null;

        return isNormalStatus;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, Name = {Name} {NumberOfRuns}ts";
    }
}
