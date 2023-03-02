using Microsoft.Extensions.Logging;

namespace OpenJob.Processors;

public class TestProcessor : ProcessorBase
{
    public TestProcessor(ILoggerFactory loggerFactory) : base(loggerFactory) { }

    protected override Task<ProcessorResult> DoWorkAsync(ProcessorContext context)
    {
        Logger.LogInformation("TestProcessor runing...");

        return Task.FromResult(ProcessorResult.OK);
    }
}
