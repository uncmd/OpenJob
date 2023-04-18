namespace OpenJob.Model;

public class ProcessorReq
{
    public string AppName { get; set; }

    public Guid AppId { get; set; }

    public string ModuleName { get; set; }

    public string Version { get; set; }

    public List<ProcessorReqDtl> Dtls { get; set; }
}

public class ProcessorReqDtl
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string ProcessorInfo { get; set; }

    public bool IsEnabled { get; set; }
}
