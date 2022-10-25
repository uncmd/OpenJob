﻿using PowerScheduler.Enums;
using Volo.Abp.Application.Dtos;

namespace PowerScheduler.Services.Dtos;

public class SchedulerTaskDto : AuditedEntityDto<Guid>
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
