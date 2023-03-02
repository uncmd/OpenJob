using Microsoft.Extensions.Logging;

namespace OpenJob.Processors;

public class OpenJobProcessorLoader : IProcessorLoader
{
    private readonly List<IProcessorFactory> _processorFactories;
    private readonly ILogger<OpenJobProcessorLoader> _logger;

    public OpenJobProcessorLoader(
        IEnumerable<IProcessorFactory> processorFactories,
        ILogger<OpenJobProcessorLoader> logger)
    {
        _processorFactories = processorFactories.ToList();
        _logger = logger;

        foreach (var processorFactory in _processorFactories)
        {
            _logger.LogInformation("register ProcessorFactory: {ProcessorFactory}", processorFactory);
        }
    }

    public IProcessor Load(ProcessorDefinition definition)
    {
        _logger.LogInformation("start to load Processor: {ProcessorDefinition}", definition);

        var processorType = definition.ProcessorType;
        foreach (var processorFactory in _processorFactories)
        {
            if (!processorFactory.SupportTypes().Contains(processorType))
            {
                _logger.LogInformation("[{ProcessorFactory}] can't load type={ProcessorType}, skip!", processorFactory, processorType);
                continue;
            }

            _logger.LogInformation("[{ProcessorFactory}] try to load processor: {ProcessorDefinition}", processorFactory, definition);

            try
            {
                var processor = processorFactory.Build(definition);
                if (processor != null)
                {
                    _logger.LogInformation("[{ProcessorFactory}] load processor successfully: {ProcessorDefinition}", processorFactory, definition);
                    return processor;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{ProcessorFactory}] load processor failed: {ProcessorDefinition}", processorFactory, definition);
                throw;
            }
        }

        throw new OpenJobException("fetch Processor failed, please check your processorType and processorInfo config");
    }
}
