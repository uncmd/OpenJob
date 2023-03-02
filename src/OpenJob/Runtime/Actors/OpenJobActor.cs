using Microsoft.Extensions.Options;
using OpenJob.Entities;
using Orleans.Concurrency;
using Orleans.Runtime;
using Volo.Abp.Domain.Repositories;

namespace OpenJob.Runtime.Actors;

public class OpenJobActor : ActorBase, IOpenJobActor, IRemindable
{
    private string versionOrleans;
    private string versionHost;
    private IGrainReminder _grainReminder;

    private readonly OpenJobOptions _options;
    private readonly IRepository<AppInfo, Guid> _appRepository;

    public OpenJobActor(
        IOptions<OpenJobOptions> options,
        IRepository<AppInfo, Guid> appRepository)
    {
        _options = options.Value;
        _appRepository = appRepository;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);

        Logger.LogInformation("OpenJob activate, it will register a timer period: {SchedulePeriod}", _options.SchedulePeriod);

        RegisterTimer(Schedule, null, TimeSpan.FromSeconds(5), _options.SchedulePeriod);
    }

    protected virtual async Task Schedule(object state)
    {
        var apps = await _appRepository.GetListAsync(p => p.IsEnabled);
        if (!apps.Any())
        {
            Logger.LogInformation("current no app to schedule.");
            return;
        }

        List<Task> tasks = new List<Task>();
        foreach (var app in apps)
        {
            var jobActor = GrainFactory.GetGrain<ISchedulerJobActor>(app.Id);
            var task = jobActor.Schedule(app.Id);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }

    public Task SetVersion(string orleans, string host)
    {
        versionOrleans = orleans;
        versionHost = host;

        return Task.CompletedTask;
    }

    public Task<Immutable<Dictionary<string, string>>> GetExtendedProperties()
    {
        var results = new Dictionary<string, string>
        {
            ["HostVersion"] = versionHost,
            ["OrleansVersion"] = versionOrleans
        };

        return Task.FromResult(results.AsImmutable());
    }

    public async Task Start()
    {
        _grainReminder = await this.GetReminder(OpenJobConsts.SchedulerReminderName);

        if (_grainReminder == null)
        {
            var dueTime = TimeSpan.FromSeconds(5);
            var period = TimeSpan.FromMinutes(1);

            Logger.LogInformation("Register OpenJob Reminder: {ReminderName}, dueTime: {DueTime}, period: {Period}", OpenJobConsts.SchedulerReminderName, dueTime, period);

            _grainReminder = await this.RegisterOrUpdateReminder(OpenJobConsts.SchedulerReminderName, dueTime, period);
        }
    }

    public async Task Stop()
    {
        if (_grainReminder != null)
        {
            Logger.LogInformation("Unregister OpenJob Reminder: {ReminderName}", _grainReminder.ReminderName);

            await this.UnregisterReminder(_grainReminder);
            _grainReminder = null;
        }
    }

    public Task ReceiveReminder(string reminderName, TickStatus status)
    {
        Logger.LogInformation("ReceiveReminder {ReminderName}: {Status}", reminderName, status);

        return Task.CompletedTask;
    }
}
