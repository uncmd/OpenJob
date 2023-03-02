using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenJob.Enums;

namespace OpenJob.Processors;

public abstract class ProcessorFactoryBase : IProcessorFactory
{
    protected ILoggerFactory LoggerFactory { get; }

    protected ILogger Logger => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance;

    public ProcessorFactoryBase(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory;
    }

    public abstract IProcessor Build(ProcessorDefinition processorDefinition);

    public abstract List<ProcessorType> SupportTypes();
}
