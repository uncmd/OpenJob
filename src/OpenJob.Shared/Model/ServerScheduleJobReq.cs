using OpenJob.Enums;

namespace OpenJob.Model;

/// <summary>
/// 服务端调度任务请求
/// </summary>
[Serializable]
public class ServerScheduleJobReq
{
    /// <summary>
    /// 可用处理器地址
    /// </summary>
    public Dictionary<string, string> ConnectionIdAddress { get; set; }

    /// <summary>
    /// 任务ID，当更换Server后需要根据 JobId 重新查询任务元数据
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    public string JobName { get; set; }

    /// <summary>
    /// 任务实例ID
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// 任务执行类型，单机、广播、MR
    /// </summary>
    public ExecutionMode ExecutionMode { get; set; }

    /// <summary>
    /// 处理器类型
    /// </summary>
    public ProcessorType ProcessorType { get; set; }

    /// <summary>
    /// 处理器信息
    /// </summary>
    public string ProcessorInfo { get; set; }

    /// <summary>
    /// 超时时间
    /// </summary>
    public int TimeoutSecond { get; set; }

    /// <summary>
    /// 任务级别的参数
    /// </summary>
    public string JobArgs { get; set; }

    /// <summary>
    /// 实例级别的参数
    /// </summary>
    public string TaskArgs { get; set; }

    /// <summary>
    /// 子任务重试次数（任务本身的重试机制由server控制）
    /// </summary>
    public int TaskRetryNum { get; set; }

    /// <summary>
    /// 时间表达式类型（CRON/API/FIX_RATE/Delayed）
    /// </summary>
    public TimeExpression TimeExpression { get; set; }

    /// <summary>
    /// 时间表达式，CRON/NULL/LONG/LONG（单位S）
    /// </summary>
    public string TimeExpressionValue { get; set; }

    /// <summary>
    /// 下发策略
    /// </summary>
    public DispatchStrategy DispatchStrategy { get; set; }

    public override string ToString()
    {
        return $"JobName:{JobName},JobId:{JobId},TaskId:{TaskId}";
    }
}
