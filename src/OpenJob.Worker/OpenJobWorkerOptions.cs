namespace OpenJob;

/// <summary>
/// Worker配置选项
/// </summary>
public sealed class OpenJobWorkerOptions
{
    /// <summary>
    /// 应用名称
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// 服务地址
    /// </summary>
    public string ServerAddress { get; set; }

    public int MaxResultLength { get; set; } = 8096;

    public object UserContext { get; set; }

    public string Tag { get; set; }

    /// <summary>
    /// 心跳报告间隔，默认10S
    /// </summary>
    public int HealthReportInterval { get; set; } = 10;
}
