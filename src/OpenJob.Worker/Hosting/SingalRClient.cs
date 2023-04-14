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

public class SingalRClient : IAsyncDisposable
{
    private readonly HubConnection connection;
    private readonly OpenJobWorkerOptions workerOptions;
    private readonly ILogger<WorkerHost> _logger;

    public SingalRClient(
        IOptions<OpenJobWorkerOptions> options,
        ILogger<WorkerHost> logger)
    {
        workerOptions = options.Value;
        _logger = logger;

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
        connection.On<ServerScheduleJobReq>(nameof(IWorkerClient.RunJob), req =>
        {
            _logger.LogInformation("Receive runjob: {Req}", req);
        });

        connection.On<Guid>(nameof(IWorkerClient.StopJob), taskId =>
        {
            _logger.LogInformation("Receive stopJob: {TaskId}", taskId);
        });
    }

    /// <summary>
    /// 校验应用是否可用，必须先创建应用才能运行Worker
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> AssertApp(CancellationToken cancellationToken)
    {
        var result = await connection.InvokeAsync<WrapResult>(nameof(IServerClient.AssertApp), workerOptions.AppName, cancellationToken);
        if (result.Success == false)
        {
            _logger.LogWarning(result.Message);
        }

        InitWorkerRuntime(Guid.Parse(result.Data.ToString()));

        _logger.LogInformation("Current app: {AppName} assert success.", workerOptions.AppName);

        return result.Success;
    }

    public async Task WorkerHeartbeat(WorkerHeartbeatDto heartbeatDto, CancellationToken cancellationToken = default)
    {
        await connection.SendAsync("WorkerHeartbeat", heartbeatDto, cancellationToken);
    }

    public async Task ReportInstanceStatus(InstanceStatusDto statusDto, CancellationToken cancellationToken = default)
    {
        await connection.SendAsync("ReportInstanceStatus", statusDto, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return connection.DisposeAsync();
    }

    private void InitWorkerRuntime(Guid appId)
    {
        WorkerRuntime.AppName = workerOptions.AppName;
        WorkerRuntime.AppId = appId;
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
