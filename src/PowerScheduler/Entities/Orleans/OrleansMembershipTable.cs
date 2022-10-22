namespace PowerScheduler.Entities.Orleans;

public class OrleansMembershipTable
{
    public string DeploymentId { get; set; }

    public string Address { get; set; }

    public int Port { get; set; }

    public int Generation { get; set; }

    public string SiloName { get; set; }

    public string HostName { get; set; }

    public int Status { get; set; }

    public int ProxyPort { get; set; }

    public string SuspectTimes { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime IAmAliveTime { get; set; }
}
