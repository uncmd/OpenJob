using OpenJob.Model;

namespace OpenJob.Core;

public interface IWorkerClient
{
    /// <summary>
    /// 执行任务
    /// </summary>
    /// <param name="jobReq"></param>
    /// <returns></returns>
    Task<string> RunJob(ServerScheduleJobReq jobReq);

    /// <summary>
    /// 终止任务
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns></returns>
    Task StopJob(Guid taskId);
}
