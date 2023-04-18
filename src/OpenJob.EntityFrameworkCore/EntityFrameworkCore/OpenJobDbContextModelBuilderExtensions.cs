using Microsoft.EntityFrameworkCore;
using OpenJob.Apps;
using OpenJob.Jobs;
using OpenJob.Orleans;
using OpenJob.Tasks;
using OpenJob.Workers;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace OpenJob.Data;

public static class OpenJobDbContextModelBuilderExtensions
{
    public static void ConfigureOpenJob(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<AppInfo>(b =>
        {
            b.ToTable(OpenJobConsts.DbTablePrefix + "AppInfo", OpenJobConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(p => p.Name).IsRequired().HasMaxLength(128);
            b.Property(p => p.Description).HasMaxLength(512);
            b.Property(p => p.IsEnabled);

            b.HasIndex(p => p.Name).IsUnique();
        });

        builder.Entity<JobInfo>(b =>
        {
            b.ToTable(OpenJobConsts.DbTablePrefix + "JobInfo", OpenJobConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(p => p.AppId).IsRequired();
            b.Property(p => p.Name).IsRequired().HasMaxLength(128);
            b.Property(p => p.Description).HasMaxLength(512);
            b.Property(p => p.Labels).HasMaxLength(256);
            b.Property(p => p.JobPriority);
            b.Property(p => p.JobArgs);
            b.Property(p => p.JobStatus);
            b.Property(p => p.ProcessorType);
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
            b.Property(p => p.DispatchStrategy);
            b.Property(p => p.MinCpuCores);
            b.Property(p => p.MinMemory);
            b.Property(p => p.MinDisk);
            b.Property(p => p.NumberOfRuns);
            b.Property(p => p.MaxNumberOfRuns);
            b.Property(p => p.NumberOfErrors);
            b.Property(p => p.MaxNumberOfErrors);

            b.HasIndex(p => p.AppId);
            b.HasIndex(p => p.Name);
            b.HasIndex(p => p.NextTriggerTime);
        });

        builder.Entity<TaskInfo>(b =>
        {
            b.ToTable(OpenJobConsts.DbTablePrefix + "TaskInfo", OpenJobConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(p => p.AppId).IsRequired();
            b.Property(p => p.JobId).IsRequired();
            b.Property(p => p.JobArgs);
            b.Property(p => p.TaskArgs);
            b.Property(p => p.TaskRunStatus);
            b.Property(p => p.ExpectedTriggerTime);
            b.Property(p => p.ActualTriggerTime);
            b.Property(p => p.FinishedTime);
            b.Property(p => p.WorkerHost).HasMaxLength(64);
            b.Property(p => p.Result).HasMaxLength(1024);
            b.Property(p => p.TryCount);

            b.HasIndex(p => p.AppId);
            b.HasIndex(p => p.JobId);
            b.HasIndex(p => p.ExpectedTriggerTime);
        });

        builder.Entity<WorkerInfo>(b =>
        {
            b.ToTable(OpenJobConsts.DbTablePrefix + "WorkerInfo", OpenJobConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(p => p.AppId).IsRequired();
            b.Property(p => p.Address).HasMaxLength(32);
            b.Property(p => p.ConnectionId).HasMaxLength(64);
            b.Property(p => p.LastActiveTime);
            b.Property(p => p.Client).HasMaxLength(128);
            b.Property(p => p.Labels).HasMaxLength(256);
            b.Property(p => p.CpuProcessors);
            b.Property(p => p.CpuLoad);
            b.Property(p => p.MemoryTotal);
            b.Property(p => p.MemoryUsed);
            b.Property(p => p.DiskTotal);
            b.Property(p => p.DiskUsed);
            b.Property(p => p.Score);

        });
        
        builder.Entity<OrleansQuery>(b =>
        {
            b.ToTable("OrleansQuery");

            int queryTextMaxLength = 8000;
            if (builder.GetDatabaseProvider() == EfCoreDatabaseProvider.Oracle)
            {
                queryTextMaxLength = 4000;
            }
            b.Property(p => p.QueryKey).HasMaxLength(64).IsRequired();
            b.Property(p => p.QueryText).HasMaxLength(queryTextMaxLength).IsRequired();

            b.HasKey(p => p.QueryKey);
        });

        builder.Entity<OrleansMembershipVersionTable>(b =>
        {
            b.ToTable("OrleansMembershipVersionTable");

            b.Property(p => p.DeploymentId).HasMaxLength(150).IsRequired();
            b.Property(p => p.Timestamp).IsRequired().HasColumnType("DATETIME2").HasPrecision(3).HasDefaultValueSql("getdate()");
            b.Property(p => p.Version).IsRequired().HasDefaultValue(0);

            b.HasKey(p => p.DeploymentId);
            b.HasMany(p => p.OrleansMembershipTables).WithOne().HasForeignKey(p => p.DeploymentId).IsRequired();
        });

        builder.Entity<OrleansMembershipTable>(b =>
        {
            b.ToTable("OrleansMembershipTable");

            b.Property(p => p.DeploymentId).HasMaxLength(150).IsRequired();
            b.Property(p => p.Address).HasMaxLength(45).IsRequired();
            b.Property(p => p.Port).IsRequired();
            b.Property(p => p.Generation).IsRequired();
            b.Property(p => p.SiloName).HasMaxLength(150).IsRequired();
            b.Property(p => p.HostName).HasMaxLength(150).IsRequired();
            b.Property(p => p.Status).IsRequired();
            b.Property(p => p.ProxyPort).IsRequired();
            b.Property(p => p.SuspectTimes).HasMaxLength(8000);
            b.Property(p => p.StartTime).IsRequired().HasColumnType("DATETIME2").HasPrecision(3);
            b.Property(p => p.IAmAliveTime).IsRequired().HasColumnType("DATETIME2").HasPrecision(3);

            b.HasKey(p => new { p.DeploymentId, p.Address, p.Port, p.Generation });
        });

        builder.Entity<OrleansRemindersTable>(b =>
        {
            b.ToTable("OrleansRemindersTable");

            b.Property(p => p.ServiceId).HasMaxLength(150).IsRequired();
            b.Property(p => p.GrainId).HasMaxLength(150).IsRequired();
            b.Property(p => p.ReminderName).HasMaxLength(150).IsRequired();
            b.Property(p => p.StartTime).IsRequired().HasColumnType("DATETIME2").HasPrecision(3);
            b.Property(p => p.Period).IsRequired();
            b.Property(p => p.GrainHash).IsRequired();
            b.Property(p => p.Version).IsRequired();

            b.HasKey(p => new { p.ServiceId, p.GrainId, p.ReminderName });
        });

        builder.Entity<OrleansStorage>(b =>
        {
            b.ToTable("OrleansStorage");

            b.Property(p => p.GrainIdHash).IsRequired();
            b.Property(p => p.GrainIdN0).IsRequired();
            b.Property(p => p.GrainIdN1).IsRequired();
            b.Property(p => p.GrainTypeHash).IsRequired();
            b.Property(p => p.GrainTypeString).HasMaxLength(512).IsRequired();
            b.Property(p => p.GrainIdExtensionString).HasMaxLength(512);
            b.Property(p => p.ServiceId).HasMaxLength(150).IsRequired();
            b.Property(p => p.PayloadBinary);
            b.Property(p => p.ModifiedOn).IsRequired().HasColumnType("DATETIME2").HasPrecision(3);
            b.Property(p => p.Version);

            b.HasNoKey();
            b.HasIndex(p => p.GrainIdHash).IsClustered(false);
            b.HasIndex(p => p.GrainTypeHash).IsClustered(false);

            // ALTER TABLE OrleansStorage SET(LOCK_ESCALATION = DISABLE);

            // IF EXISTS (SELECT 1 FROM sys.dm_db_persisted_sku_features WHERE feature_id = 100)
            // BEGIN
            //     ALTER TABLE OrleansStorage REBUILD PARTITION = ALL WITH(DATA_COMPRESSION = PAGE);
            // END
        });
    }
}
