using Microsoft.Extensions.Logging;

namespace PowerScheduler.Processors;

public class TestProcessor : ProcessorBase
{
    protected override Task<ProcessorResult> DoWorkAsync(ProcessorContext context)
    {
        Logger.LogInformation("TestProcessor runing...");

        return Task.FromResult(ProcessorResult.OK);
    }
}
