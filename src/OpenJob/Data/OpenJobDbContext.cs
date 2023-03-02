using Microsoft.EntityFrameworkCore;
using OpenJob.Entities;
using OpenJob.Entities.Orleans;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace OpenJob.Data;

public class OpenJobDbContext : AbpDbContext<OpenJobDbContext>
{
    public OpenJobDbContext(DbContextOptions<OpenJobDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppInfo> AppInfos { get; set; }
    public DbSet<JobInfo> JobInfos { get; set; }
    public DbSet<TaskInfo> TaskInfos { get; set; }
    public DbSet<WorkerInfo> WorkerInfos { get; set; }

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
        builder.ConfigureOpenJob();
    }
}
