using OpenJob.Model;

namespace OpenJob.Core;

public interface IServerClient
{
    /// <summary>
    /// App 校验
    /// </summary>
    /// <param name="appName">应用名称，唯一</param>
    /// <returns></returns>
    Task<WrapResult> AssertApp(string appName);

    /// <summary>
    /// 心跳
    /// </summary>
    /// <param name="heartbeatDto"></param>
    /// <returns></returns>
    Task<WrapResult> WorkerHeartbeat(WorkerHeartbeatDto heartbeatDto);

    /// <summary>
    /// 任务实例状态上报
    /// </summary>
    /// <returns></returns>
    Task<WrapResult> ReportInstanceStatus(InstanceStatusDto statusDto);

    /// <summary>
    /// 在线日志
    /// </summary>
    /// <returns></returns>
    Task<WrapResult> ReportLog();
}
