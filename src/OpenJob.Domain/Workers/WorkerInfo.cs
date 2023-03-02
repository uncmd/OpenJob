using Volo.Abp.Domain.Entities.Auditing;

namespace OpenJob.Workers;

public sealed class WorkerInfo : AuditedAggregateRoot<Guid>, IComparable<WorkerInfo>
{
    public Guid AppId { get; set; }

    public string Address { get; set; }

    public DateTime LastActiveTime { get; set; }

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

    /// <summary>
    /// 机器分数
    /// </summary>
    public int Score { get; set; }

    private WorkerInfo() { }

    public WorkerInfo(Guid id, Guid appId) : base(id)
    {
        AppId = appId;
    }

    /// <summary>
    /// 计算机器分数
    /// </summary>
    /// <returns></returns>
    public int CalculateScore()
    {
        var memoryScore = (MemoryTotal - MemoryUsed) * 2;
        var cpuScore = CpuProcessors - CpuLoad;

        // Windows can not fetch CPU load, set cpuScore as 1.
        if (cpuScore > CpuProcessors)
        {
            cpuScore = 1;
        }

        Score = (int)(memoryScore + cpuScore);

        return Score;
    }

    /// <summary>
    /// 判断机器是否可用
    /// </summary>
    /// <param name="minCPUCores"></param>
    /// <param name="minMemorySpace"></param>
    /// <param name="minDiskSpace"></param>
    /// <returns>机器是否可用</returns>
    public bool Available(double minCPUCores, double minMemorySpace, double minDiskSpace)
    {
        double availableMemory = MemoryTotal - MemoryUsed;
        double availableDisk = DiskTotal - DiskUsed;

        if (availableMemory < minMemorySpace || availableDisk < minDiskSpace)
        {
            return false;
        }

        // 0 indicates the CPU is free, which is the optimal condition.
        // Negative number means being unable to fetch CPU info, return true.
        if (CpuLoad <= 0 || minCPUCores <= 0)
        {
            return true;
        }
        return minCPUCores < CpuProcessors - CpuLoad;
    }

    public bool Timeout()
    {
        return DateTime.Now - LastActiveTime > TimeSpan.FromMinutes(1);
    }

    public string PrintInfo() => $"cpu: {CpuLoad}/{CpuProcessors} mem: {MemoryUsed}/{MemoryTotal} disk: {DiskUsed}/{DiskTotal}";

    public int CompareTo(WorkerInfo other)
    {
        // 按分数降序排序
        return other.CalculateScore() - CalculateScore();
    }

    public override string ToString()
    {
        return $"{base.ToString()}, Address = {Address}";
    }
}
