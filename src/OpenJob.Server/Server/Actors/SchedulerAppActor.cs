using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenJob.Apps;
using Volo.Abp.Domain.Repositories;

namespace OpenJob.Server.Actors;

public class SchedulerAppActor : ActorBase<long>, ISchedulerAppActor
{
    private readonly OpenJobServerOptions _options;
    private readonly IRepository<AppInfo, Guid> _appRepository;
    private IDisposable _timer;

    public SchedulerAppActor(
        IOptions<OpenJobServerOptions> options,
        IRepository<AppInfo, Guid> appRepository)
    {
        _options = options.Value;
        _appRepository = appRepository;
    }

    public Task KeepAlive()
    {
        _timer ??= RegisterTimer(SchedulerApp, null, TimeSpan.FromSeconds(1), _options.SchedulePeriod);
        return Task.CompletedTask;
    }

    private async Task SchedulerApp(object state)
    {
        try
        {
            var apps = await _appRepository.GetListAsync(p => p.IsEnabled);
            if (!apps.Any())
            {
                Logger.LogInformation(L["NoScheduleApp"]);
                return;
            }

            List<Task> tasks = new List<Task>();
            foreach (var app in apps)
            {
                var jobActor = GrainFactory.GetGrain<ISchedulerJobActor>(app.Id);
                var task = jobActor.ScheduleJob(app.Id, app.Name);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error in {nameof(SchedulerApp)}");
        }
    }
}
