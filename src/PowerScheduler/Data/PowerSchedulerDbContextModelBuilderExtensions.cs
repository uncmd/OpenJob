using Microsoft.EntityFrameworkCore;
using PowerScheduler.Entities;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace PowerScheduler.Data;

public static class PowerSchedulerDbContextModelBuilderExtensions
{
    public static void ConfigurePowerScheduler(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<SchedulerJob>(b =>
        {
            b.ToTable(PowerSchedulerDbProperties.DbTablePrefix + "Job", PowerSchedulerDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(p => p.Name).HasMaxLength(128);
            b.Property(p => p.Description).HasMaxLength(512);
            b.Property(p => p.Labels).HasMaxLength(256);
            b.Property(p => p.JobPriority);
            b.Property(p => p.JobArgs).HasMaxLength(512);
            b.Property(p => p.IsEnabled);
            b.Property(p => p.IsAbandoned);
            b.Property(p => p.JobType);
            b.Property(p => p.ExecutionMode);
            b.Property(p => p.ProcessorInfo).HasMaxLength(256);
            b.Property(p => p.TimeExpression);
            b.Property(p => p.TimeExpressionValue).HasMaxLength(256);
            b.Property(p => p.BeginTime);
            b.Property(p => p.EndTime);
            b.Property(p => p.MaxTryCount);
            b.Property(p => p.TimeoutSecond);
            b.Property(p => p.NextTriggerTime);
            b.Property(p => p.LastTriggerTime);
            b.Property(p => p.MisfireStrategy);

            b.HasMany(p => p.SchedulerTasks).WithOne().HasForeignKey(p => p.JobId).IsRequired();
        });

        builder.Entity<SchedulerTask>(b =>
        {
            b.ToTable(PowerSchedulerDbProperties.DbTablePrefix + "Task", PowerSchedulerDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(p => p.JobId);
            b.Property(p => p.TaskArgs).HasMaxLength(512);
            b.Property(p => p.TaskRunStatus);
            b.Property(p => p.ExpectedTriggerTime);
            b.Property(p => p.ActualTriggerTime);
            b.Property(p => p.FinishedTime);
            b.Property(p => p.WorkerHost).HasMaxLength(64);
            b.Property(p => p.Result).HasMaxLength(1024);
            b.Property(p => p.TryCount);

            b.HasIndex(p => p.JobId);
        });
    }
}
