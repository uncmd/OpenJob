namespace OpenJob.Workers;

public class CreateUpdateWorkerDto
{
    public string Address { get; set; }

    public string Client { get; set; }

    public string Labels { get; set; }

    public int CpuProcessors { get; set; }

    public double CpuLoad { get; set; }

    /// <summary>
    /// 已使用内存 GB
    /// </summary>
    public double MemoryUsed { get; set; }

    /// <summary>
    /// 总内存 GB
    /// </summary>
    public double MemoryTotal { get; set; }

    /// <summary>
    /// 已使用硬盘 GB
    /// </summary>
    public double DiskUsed { get; set; }

    /// <summary>
    /// 总硬盘 GB
    /// </summary>
    public double DiskTotal { get; set; }
}
