using Microsoft.Extensions.Options;
using Orleans.Concurrency;
using Orleans.Runtime;
using PowerScheduler.Actors;
using PowerScheduler.Entities;
using Volo.Abp.Domain.Repositories;

namespace PowerScheduler.Runtime;

public class PowerSchedulerActor : ActorBase, IPowerSchedulerActor, IRemindable
{
    private string versionOrleans;
    private string versionHost;
    private IGrainReminder _grainReminder;

    private readonly PowerSchedulerOptions _options;
    private readonly IRepository<SchedulerApp, Guid> _appRepository;

    public PowerSchedulerActor(
        IOptions<PowerSchedulerOptions> options,
        IRepository<SchedulerApp, Guid> appRepository)
    {
        _options = options.Value;
        _appRepository = appRepository;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);

        Logger.LogInformation("PowerScheduler activate, it will register a timer period: {SchedulePeriod}", _options.SchedulePeriod);

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
        _grainReminder = await this.GetReminder(PowerSchedulerConsts.SchedulerReminderName);

        if (_grainReminder == null)
        {
            var dueTime = TimeSpan.FromSeconds(5);
            var period = TimeSpan.FromMinutes(1);

            Logger.LogInformation("Register PowerScheduler Reminder: {ReminderName}, dueTime: {DueTime}, period: {Period}", PowerSchedulerConsts.SchedulerReminderName, dueTime, period);

            _grainReminder = await this.RegisterOrUpdateReminder(PowerSchedulerConsts.SchedulerReminderName, dueTime, period);
        }
    }

    public async Task Stop()
    {
        if (_grainReminder != null)
        {
            Logger.LogInformation("Unregister PowerScheduler Reminder: {ReminderName}", _grainReminder.ReminderName);

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
