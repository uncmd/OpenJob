namespace OpenJob;

public static class OpenJobConsts
{
    public const string DbTablePrefix = "OpenJob";
    public const string DbSchema = null;

    public const string SchedulerReminderName = "OpenJobReminder";

    public const string EmptyAddress = "N/A";

    public const string NoWorkerAvailable = "no worker available";

    public const string HandlerRunJob = "runJob";
    public const string HandlerStopTask = "stopTask";
    public const string HandlerQueryTaskStatus = "queryTaskStatus";

    public const string Heartbeat = "heartbeat";

    public const int DefaultWorkerPort = 16666;
}
