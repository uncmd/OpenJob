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

    public async Task RunJob(ServerScheduleJobReq jobReq)
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
            JobName = jobReq.JobName,
            MaxTryCount = jobReq.TaskRetryNum,
            TaskArgs = jobReq.TaskArgs,
            TaskId = jobReq.TaskId,
            TryCount = jobReq.TaskRetryNum
        });
    }

    public Task StopJob(Guid taskId)
    {
        return Task.CompletedTask;
    }
}
