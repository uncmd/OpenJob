using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using OpenJob.Data;
using Volo.Abp.AspNetCore;
using Volo.Abp.AspNetCore.TestBase;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.VirtualFileSystem;

namespace OpenJob;

[DependsOn(
    typeof(AbpAspNetCoreTestBaseModule),
    typeof(AbpAspNetCoreModule),
    typeof(AbpAutofacModule),
    typeof(OpenJobModule),
    typeof(AbpEntityFrameworkCoreSqliteModule)
    )]
public class OpenJobTestModule : AbpModule
{
    private SqliteConnection _sqliteConnection;

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<OpenJobTestModule>();
            options.FileSets.ReplaceEmbeddedByPhysical<OpenJobTestModule>(FindProjectPath(hostingEnvironment));
        });

        ConfigureInMemorySqlite(context.Services);

        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });

        context.Services.AddAlwaysAllowAuthorization();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        SeedTestData(context);

        var app = context.GetApplicationBuilder();

        app.UseCorrelationId();
        app.UseStaticFiles();
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        _sqliteConnection.Dispose();
    }

    private string FindProjectPath(IWebHostEnvironment hostEnvironment)
    {
        var directory = new DirectoryInfo(hostEnvironment.ContentRootPath);

        while (directory != null && directory.Name != "OpenJob.Tests")
        {
            directory = directory.Parent;
        }

        return directory?.FullName
               ?? throw new Exception("Could not find the project path by beginning from " + hostEnvironment.ContentRootPath + ", going through to parents and looking for Volo.Abp.AspNetCore.Tests");
    }

    private static void SeedTestData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            using (var scope = context.ServiceProvider.CreateScope())
            {
                await scope.ServiceProvider
                    .GetRequiredService<IDataSeeder>()
                    .SeedAsync();
            }
        });
    }

    private void ConfigureInMemorySqlite(IServiceCollection services)
    {
        _sqliteConnection = CreateDatabaseAndGetConnection();

        services.Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(context =>
            {
                context.DbContextOptions.UseSqlite(_sqliteConnection);
            });
        });
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<OpenJobDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new OpenJobDbContext(options))
        {
            context.GetService<IRelationalDatabaseCreator>().CreateTables();
        }

        return connection;
    }
}
