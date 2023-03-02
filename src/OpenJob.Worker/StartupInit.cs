using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace OpenJob;

public class StartupInit
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<StartupInit> _logger;
    private readonly ServerClientFactory _serverClient;

    public StartupInit(IConfiguration configuration, ILogger<StartupInit> logger, ServerClientFactory serverClient)
    {
        _configuration = configuration;
        _logger = logger;
        _serverClient = serverClient;
    }

    public async Task OnPostApplicationInitializationAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("start to initialize OpenJobWorker...");

        var options = new OpenJobWorkerOptions();
        _configuration.GetSection("WorkerOptions").Bind(options);

        try
        {
            await AssertAppName(options);

            WorkerRuntime.WorkerAddress = $"{GetLocalIp}:{options.Port}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "initialize PowerJobWorker failed, using {Elapsed}.", stopwatch);
            throw;
        }
    }

    private async Task AssertAppName(OpenJobWorkerOptions workerOptions)
    {
        if (string.IsNullOrWhiteSpace(workerOptions.AppName))
        {
            throw new OpenJobException("AppName can't be empty!");
        }

        var appId = await _serverClient.AssertAppName($"assert?appName={workerOptions.AppName}");
        WorkerRuntime.AppId = appId;
    }

    /// <summary>
    /// 获取本地Ip地址
    /// </summary>
    /// <returns></returns>
    public static string GetLocalIp()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList
            .FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork)
            ?.ToString();
    }
}
