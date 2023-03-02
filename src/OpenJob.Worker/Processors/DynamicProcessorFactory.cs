using Microsoft.Extensions.Logging;
using OpenJob.Enums;

namespace OpenJob.Processors;

public class DynamicProcessorFactory : ProcessorFactoryBase
{
    public DynamicProcessorFactory(ILoggerFactory loggerFactory) : base(loggerFactory) { }

    public override List<ProcessorType> SupportTypes()
    {
        return new List<ProcessorType> { ProcessorType.Dynamic };
    }

    public override IProcessor Build(ProcessorDefinition processorDefinition)
    {
        // 动态加载上传的程序集
        throw new NotImplementedException();
    }
}
