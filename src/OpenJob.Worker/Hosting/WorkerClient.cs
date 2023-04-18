using OpenJob.Core;
using OpenJob.Model;
using OpenJob.Processors;

namespace OpenJob.Hosting;

public class WorkerClient : IWorkerClient
{
    private readonly IProcessorLoader _processorLoader;

    public WorkerClient(IProcessorLoader processorLoader)
    {
        _processorLoader = processorLoader;
    }

    public async Task<string> RunJob(ServerScheduleJobReq jobReq)
    {
        var processor = _processorLoader.Load(new ProcessorDefinition
        {
            ProcessorType = jobReq.ProcessorType,
            ProcessorInfo = jobReq.ProcessorInfo,
        });

        await processor.ExecuteAsync(new ProcessorContext
        {
            JobArgs = jobReq.JobArgs,
            JobId = jobReq.JobId,
            MaxTryCount = jobReq.TaskRetryNum,
            TaskArgs = jobReq.TaskArgs,
            TaskId = jobReq.TaskId,
            TryCount = jobReq.TaskRetryNum
        });

        return string.Empty;
    }

    public Task StopJob(Guid taskId)
    {
        throw new NotImplementedException();
    }
}
