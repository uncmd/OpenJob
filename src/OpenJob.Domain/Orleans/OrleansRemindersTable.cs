namespace OpenJob.Orleans;

public class OrleansRemindersTable
{
    public string ServiceId { get; set; }

    public string GrainId { get; set; }

    public string ReminderName { get; set; }

    public DateTime StartTime { get; set; }

    public long Period { get; set; }

    public int GrainHash { get; set; }

    public int Version { get; set; }
}
