namespace OpenJob.Model;

[Serializable]
public class SystemMetrics : IComparable<SystemMetrics>
{
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

    public int CompareTo(SystemMetrics other)
    {
        // 按分数降序排序
        return other.CalculateScore() - CalculateScore();
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
}
