namespace OpenJob.Orleans;

public class OrleansStorage
{
    public int GrainIdHash { get; set; }

    public long GrainIdN0 { get; set; }

    public long GrainIdN1 { get; set; }

    public int GrainTypeHash { get; set; }

    public string GrainTypeString { get; set; }

    public string GrainIdExtensionString { get; set; }

    public string ServiceId { get; set; }

    public byte[] PayloadBinary { get; set; }

    public DateTime ModifiedOn { get; set; }

    public int Version { get; set; }
}
