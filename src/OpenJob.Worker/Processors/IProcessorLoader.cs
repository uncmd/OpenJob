namespace OpenJob.Processors;

/// <summary>
/// 内部使用的 Processor 加载器
/// </summary>
public interface IProcessorLoader
{
    IProcessor Load(ProcessorDefinition definition);
}
