namespace OpenJob.Orleans;

public class OrleansMembershipVersionTable
{
    public string DeploymentId { get; set; }

    public DateTime Timestamp { get; set; }

    public int Version { get; set; }

    public ICollection<OrleansMembershipTable> OrleansMembershipTables { get; set; }
}
