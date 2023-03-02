using Microsoft.Extensions.Options;
using OpenJob.Enums;
using static OpenJob.OpenJobConsts;

namespace OpenJob;

public class ServerClientFactory
{
    private readonly OpenJobClient _client;

    public ServerClientFactory(IOptions<OpenJobWorkerOptions> options)
    {
        _client = new OpenJobClientBuilder()
            .UseHttpEndpoint(options.Value.GatewayAddress)
            .Build();
    }

    public Task<Guid> AssertAppName(string methodName)
    {
        return _client.InvokeMethodAsync<Guid>(ServerType.Server, methodName);
    }

    public Task WorkerHeartbeat(object data)
    {
        return _client.InvokeMethodAsync(ServerType.Server, Heartbeat, data);
    }
}
