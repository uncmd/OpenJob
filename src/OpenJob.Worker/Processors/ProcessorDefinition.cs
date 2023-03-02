using OpenJob.Enums;
using System.Diagnostics.CodeAnalysis;

namespace OpenJob.Processors;

/// <summary>
/// 处理器定义
/// </summary>
public class ProcessorDefinition : IEqualityComparer<ProcessorDefinition>
{
    /// <summary>
    /// 处理器类型
    /// </summary>
    public ProcessorType ProcessorType { get; set; }

    /// <summary>
    /// 处理器信息
    /// 命名空间.类型名,程序集
    /// </summary>
    public string ProcessorInfo { get; set; }

    public bool Equals(ProcessorDefinition x, ProcessorDefinition y)
    {
        if (x == null || y == null)
            return false;

        return x.ProcessorType.Equals(y.ProcessorType) && x.ProcessorInfo.Equals(y.ProcessorInfo);
    }

    public int GetHashCode([DisallowNull] ProcessorDefinition obj)
    {
        return ProcessorType.GetHashCode() + ProcessorInfo.GetHashCode();
    }

    public override string ToString()
    {
        return $"{ProcessorType}->{ProcessorInfo}";
    }
}
