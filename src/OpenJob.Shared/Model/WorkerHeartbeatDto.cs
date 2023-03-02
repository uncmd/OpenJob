namespace OpenJob.Model;

[Serializable]
public class WorkerHeartbeatDto
{
    public string WorkerAddress { get; set; }

    public string AppName { get; set; }

    public Guid AppId { get; set; }

    public DateTime HeartbeatTime { get; set; }

    public string WorkerVersion { get; set; }

    public string Tag { get; set; }

    public string Client { get; set; }

    public SystemMetrics SystemMetrics { get; set; }

    public override string ToString()
    {
        return $"AppId: {AppId}, AppName: {AppName}";
    }
}
