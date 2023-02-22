using static PowerScheduler.PowerSchedulerConsts;

namespace PowerScheduler.Domain;

/// <summary>
/// 统一生成地址
/// </summary>
public class ServerURLFactory
{
    public static Uri DispatchJob2Worker(string address)
    {
        return Build(address, WorkerPath, HandlerRunJob);
    }

    public static Uri StopTask2Worker(string address)
    {
        return Build(address, WorkerPath, HandlerStopTask);
    }

    public static Uri QueryTask2Worker(string address)
    {
        return Build(address, WorkerPath, HandlerQueryTaskStatus);
    }

    private static Uri Build(string address, string rootPath, string handlerPath)
    {
        return new Uri(new Uri(new Uri(address), rootPath), handlerPath);
    }
}
