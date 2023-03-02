using OpenJob.Enums;

namespace OpenJob.Services.Dtos;

public class CreateUpdateTaskDto
{
    public Guid JobId { get; set; }

    public string TaskArgs { get; set; }

    public TaskRunStatus TaskRunStatus { get; set; }

    public DateTimeOffset ExpectedTriggerTime { get; set; }

    public DateTimeOffset ActualTriggerTime { get; set; }

    public DateTimeOffset FinishedTime { get; set; }

    public string WorkerHost { get; set; }

    public string Result { get; set; }

    public int TryCount { get; set; }
}
