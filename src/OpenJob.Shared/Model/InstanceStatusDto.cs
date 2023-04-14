using OpenJob.Enums;

namespace OpenJob.Model;

/// <summary>
/// 任务实例状态上报
/// </summary>
public class InstanceStatusDto
{
    /// <summary>
    /// 任务ID
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// 当前任务实例ID
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// 上报时间，直接取当前时间
    /// </summary>
    public DateTime ReportTime { get; set; }

    /// <summary>
    /// 本机地址
    /// </summary>
    public string SourceAddress { get; set; }

    /// <summary>
    /// 任务状态
    /// </summary>
    public TaskRunStatus TaskRunStatus { get; set; }
}
