using OpenJob.Enums;
using static OpenJob.OpenJobConsts;

namespace OpenJob.Domain;

/// <summary>
/// 统一生成地址
/// </summary>
public class WorkerClientFactory
{
    public static Task DispatchJob2Worker(string address, object data)
    {
        return Send(address, HandlerRunJob, data);
    }

    public static Task StopTask2Worker(string address, object data)
    {
        return Send(address, HandlerStopTask, data);
    }

    public static Task QueryTask2Worker(string address, object data)
    {
        return Send(address, HandlerQueryTaskStatus, data);
    }

    public static Task Send(string httpEndpoint, string methodName, object data, CancellationToken cancellationToken = default)
    {
        var client = new OpenJobClientBuilder()
            .UseHttpEndpoint(httpEndpoint)
            .Build();

        return client.InvokeMethodAsync(ServerType.Worker, methodName, data, cancellationToken);
    }
}
