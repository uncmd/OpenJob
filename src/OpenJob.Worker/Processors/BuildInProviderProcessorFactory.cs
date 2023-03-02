using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenJob.Enums;

namespace OpenJob.Processors;

public class BuildInProviderProcessorFactory : ProcessorFactoryBase
{
    private readonly IServiceProvider _serviceProvider;

    public BuildInProviderProcessorFactory(
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
            return (IProcessor)ActivatorUtilities.CreateInstance(_serviceProvider, processorType);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "load Processor({ProcessorDefinition}) failed.", processorDefinition);
        }

        return null;
    }
}
