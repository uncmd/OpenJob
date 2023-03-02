using OpenJob.Enums;

namespace OpenJob.Processors;

public interface IProcessorFactory
{
    List<ProcessorType> SupportTypes();

    IProcessor Build(ProcessorDefinition processorDefinition);
}
