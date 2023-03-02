namespace OpenJob;

public sealed class OpenJobWorkerOptions
{
    public string AppName { get; set; }

    public int Port { get; set; } = OpenJobConsts.DefaultWorkerPort;

    public string GatewayAddress { get; set; }

    public int MaxResultLength { get; set; } = 8096;

    public object UserContext { get; set; }

    public string Tag { get; set; }

    public int HealthReportInterval { get; set; } = 10;
}
