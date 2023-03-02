using Microsoft.Extensions.Logging;
using OpenJob.Processors;

namespace OpenJob.Worker.ConsoleTest;

public class SimpleProcessor : ProcessorBase
{
    public SimpleProcessor(ILoggerFactory loggerFactory) : base(loggerFactory) { }

    protected override Task<ProcessorResult> DoWorkAsync(ProcessorContext context)
    {
        Logger.LogInformation("SimpleProcessor runing...");

        return Task.FromResult(ProcessorResult.OK);
    }
}
