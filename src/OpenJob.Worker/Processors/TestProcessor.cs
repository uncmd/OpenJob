using Microsoft.Extensions.Logging;

namespace OpenJob.Processors;

public class TestProcessor : ProcessorBase
{
    public TestProcessor() { }

    protected override Task<ProcessorResult> DoWorkAsync(ProcessorContext context)
    {
        Logger.LogInformation("TestProcessor is runing...");

        return Task.FromResult(ProcessorResult.OK);
    }
}
