using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenJob.Data;
using OpenJob.EntityFrameworkCore;
using Serilog;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Uow;

namespace OpenJob.DbMigrator;

public class DbMigratorHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IConfiguration _configuration;

    public DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime, IConfiguration configuration)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var application = await AbpApplicationFactory.CreateAsync<OpenJobDbMigratorModule>(options =>
        {
            options.Services.ReplaceConfiguration(_configuration);
            options.UseAutofac();
            options.Services.AddLogging(c => c.AddSerilog());
            options.AddDataMigrationEnvironment();
        }))
        {
            await application.InitializeAsync();

            var databaseProvider = "";
            var unitOfWork = application.ServiceProvider.GetRequiredService<UnitOfWorkManager>();
            using (var uw = unitOfWork.Begin())
            {
                var dbContext = application.ServiceProvider
                    .GetRequiredService<OpenJobDbContext>();

                databaseProvider = dbContext.Model["_Abp_DatabaseProvider"]?.ToString();
            }

            await application
                .ServiceProvider
                .GetRequiredService<OpenJobDbMigrationService>()
                .MigrateAsync(databaseProvider);

            await application.ShutdownAsync();

            _hostApplicationLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
