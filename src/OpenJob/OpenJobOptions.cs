namespace OpenJob;

public class OpenJobOptions
{
    /// <summary>
    /// 任务调度周期，默认为10秒
    /// </summary>
    public TimeSpan SchedulePeriod { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// 数据清理周期，默认为1小时
    /// </summary>
    public TimeSpan CleanDataPeriod { get; set; } = TimeSpan.FromHours(1);
}
