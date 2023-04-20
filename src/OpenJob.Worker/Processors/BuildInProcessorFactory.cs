using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenJob.Enums;

namespace OpenJob.Processors;

/// <summary>
/// 内建的默认处理器工厂，通过全限定类名从IOC加载处理器
/// </summary>
public class BuildInProcessorFactory : ProcessorFactoryBase
{
    private readonly IServiceProvider _serviceProvider;

    public BuildInProcessorFactory(
        ILoggerFactory loggerFactory, IServiceProvider serviceProvider) :
        base(loggerFactory)
    {
        _serviceProvider = serviceProvider;
    }

    public override List<ProcessorType> SupportTypes()
    {
        return new List<ProcessorType> { ProcessorType.BuildIn };
    }

    public override IProcessor Build(ProcessorDefinition processorDefinition)
    {
        var typeName = processorDefinition.ProcessorInfo;

        try
        {
            var processorType = Type.GetType(typeName);
            var processor = (IProcessor)ActivatorUtilities.CreateInstance(_serviceProvider, processorType);
            if (processor is ProcessorBase processorBase)
            {
                processorBase.ServiceProvider = _serviceProvider;
            }
            return processor;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "load Processor({ProcessorDefinition}) failed.", processorDefinition);
        }

        return null;
    }
}
