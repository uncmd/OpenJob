namespace PowerScheduler;

public class PowerSchedulerOptions
{
    public TimeSpan SchedulePeriod { get; set; } = TimeSpan.FromSeconds(10);

    public TimeSpan CleanDataPeriod { get; set; } = TimeSpan.FromHours(1);
}
