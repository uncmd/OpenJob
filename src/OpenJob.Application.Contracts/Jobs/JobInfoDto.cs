﻿using OpenJob.Enums;
using Volo.Abp.Application.Dtos;

namespace OpenJob.Jobs;

public class JobInfoDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Labels { get; set; }

    public JobPriority JobPriority { get; set; }

    public string JobArgs { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsAbandoned { get; set; }

    public ProcessorType JobType { get; set; }

    public ExecutionMode ExecutionMode { get; set; }

    public string ProcessorInfo { get; set; }

    public TimeExpression TimeExpression { get; set; }

    public string TimeExpressionValue { get; set; }

    public DateTimeOffset BeginTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public int MaxTryCount { get; set; }

    public int TimeoutSecond { get; set; }

    public DateTimeOffset NextTriggerTime { get; set; }

    public DateTimeOffset LastTriggerTime { get; set; }

    public MisfireStrategy MisfireStrategy { get; set; }
}
