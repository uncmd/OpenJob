using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenJob.Core;
using OpenJob.Model;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace OpenJob.Hosting;

public class ServerClientProxy : IAsyncDisposable, IServerClient
{
    private readonly HubConnection connection;
    private readonly OpenJobWorkerOptions workerOptions;
    private readonly ILogger<WorkerHost> _logger;
    private readonly IWorkerClient _workerClient;

    public ServerClientProxy(
        IOptions<OpenJobWorkerOptions> options,
        ILogger<WorkerHost> logger,
        IWorkerClient workerClient)
    {
        workerOptions = options.Value;
        _logger = logger;
        _workerClient = workerClient;
        InitWorkerRuntime();

        connection = new HubConnectionBuilder()
            .WithUrl($"{workerOptions.ServerAddress}/signalr-hubs/messaging")
            .WithAutomaticReconnect() // 默认0、2、10 和 30 秒重试
            .AddMessagePackProtocol()
            .Build();

        connection.Closed += async (error) =>
        {
            if (error != null)
            {
                _logger.LogError(error, "SignalR closed, try restart ...");
            }

            await Task.Delay(Random.Shared.Next(0, 5) * 1000);
            await connection.StartAsync();
        };
    }

    public async Task Connect(CancellationToken cancellationToken)
    {
        try
        {
            InitOnMethod();

            await connection.StartAsync(cancellationToken);
            _logger.LogInformation("Connection started.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Connection signalr error.");
        }
    }

    private void InitOnMethod()
    {
        connection.On<ServerScheduleJobReq>(nameof(IWorkerClient.RunJob), async req =>
        {
            _logger.LogInformation("Receive runjob: {@Req}", req);

            await _workerClient.RunJob(req);
        });

        connection.On<Guid>(nameof(IWorkerClient.StopJob), async taskId =>
        {
            _logger.LogInformation("Receive stopJob: {TaskId}", taskId);

            await _workerClient.StopJob(taskId);
        });
    }

    /// <summary>
    /// 校验应用是否可用，必须先创建应用才能运行Worker
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<WrapResult> AssertApp(string appName)
    {
        var result = await connection.InvokeAsync<WrapResult>(nameof(IServerClient.AssertApp), appName);
        if (result.Success == false)
        {
            _logger.LogWarning(result.Message);
            return result;
        }

        WorkerRuntime.AppId = Guid.Parse(result.Data.ToString());

        _logger.LogInformation("Current app: {AppName} assert success.", workerOptions.AppName);

        return result;
    }

    public async Task WorkerHeartbeat(WorkerHeartbeatDto heartbeatDto)
    {
        heartbeatDto.ConnectionId = connection.ConnectionId;
        await connection.SendAsync(nameof(IServerClient.WorkerHeartbeat), heartbeatDto);
    }

    public async Task<WrapResult> ReportInstanceStatus(InstanceStatusDto statusDto)
    {
        var result = await connection.InvokeAsync<WrapResult>(nameof(IServerClient.ReportInstanceStatus), statusDto);

        return result;
    }

    public async Task ReportLog()
    {
        await connection.SendAsync(nameof(IServerClient.ReportLog));
    }

    public async Task RegisterProcessor(ProcessorReq req)
    {
        await connection.SendAsync(nameof(IServerClient.RegisterProcessor), req);
    }

    public ValueTask DisposeAsync()
    {
        return connection.DisposeAsync();
    }

    private void InitWorkerRuntime()
    {
        WorkerRuntime.AppName = workerOptions.AppName;
        WorkerRuntime.Tag = workerOptions.Tag;
        WorkerRuntime.Address = GetLocalIp();
        WorkerRuntime.Version = GetVersion();
        WorkerRuntime.Client = GetClient();
    }

    /// <summary>
    /// 获取本地Ip地址
    /// </summary>
    /// <returns></returns>
    private static string GetLocalIp() => Dns.GetHostEntry(Dns.GetHostName())
        .AddressList
        .FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork)
        ?.ToString();

    private string GetVersion() => GetType().Assembly.GetName().Version.ToString();

    public static string GetClient() => Assembly.GetEntryAssembly().GetName().Name;
}
