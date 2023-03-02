namespace OpenJob.Enums;

/// <summary>
/// 处理器类型
/// </summary>
public enum ProcessorType
{
    /// <summary>
    /// 内建处理器
    /// </summary>
    BuildIn,

    /// <summary>
    /// 动态加载
    /// </summary>
    Dynamic,

    /// <summary>
    /// Http
    /// </summary>
    Http,

    /// <summary>
    /// Shell脚本
    /// </summary>
    Shell,

    /// <summary>
    /// Python脚本
    /// </summary>
    Python,

    /// <summary>
    /// Go脚本
    /// </summary>
    Go,

    /// <summary>
    /// Sql脚本
    /// </summary>
    Sql
}
