using Microsoft.EntityFrameworkCore;
using PowerScheduler.Entities;
using PowerScheduler.Entities.Orleans;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace PowerScheduler.Data;

public class PowerSchedulerDbContext : AbpDbContext<PowerSchedulerDbContext>
{
    public PowerSchedulerDbContext(DbContextOptions<PowerSchedulerDbContext> options)
        : base(options)
    {
    }

    public DbSet<SchedulerApp> SchedulerApps { get; set; }
    public DbSet<SchedulerJob> SchedulerJobs { get; set; }
    public DbSet<SchedulerTask> SchedulerTasks { get; set; }
    public DbSet<SchedulerWorker> SchedulerWorkers { get; set; }

    public DbSet<OrleansQuery> OrleansQueries { get; set; }
    public DbSet<OrleansMembershipVersionTable> OrleansMembershipVersions { get; set; }
    public DbSet<OrleansRemindersTable> OrleansRemindersTables { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own entities here */
        builder.ConfigurePowerScheduler();
    }
}
