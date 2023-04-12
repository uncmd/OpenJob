using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Runtime;

namespace OpenJob.Server.Actors;

public class SchedulerReminder : ActorBase<long>, ISchedulerReminder, IRemindable
{
    private string versionOrleans;
    private string versionHost;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);

        var dueTime = TimeSpan.FromSeconds(1);
        var period = TimeSpan.FromMinutes(3);

        Logger.LogInformation("Register OpenJob Reminder: {ReminderName}, dueTime: {DueTime}, period: {Period}", OpenJobConsts.SchedulerReminderName, dueTime, period);

        await this.RegisterOrUpdateReminder(OpenJobConsts.SchedulerReminderName, dueTime, period);
    }

    public Task Active()
    {
        return Task.CompletedTask;
    }

    public async Task ReceiveReminder(string reminderName, TickStatus status)
    {
        await KeepTimerAliveAsync();
    }

    private async Task KeepTimerAliveAsync()
    {
        await GrainFactory.GetGrain<ISchedulerAppActor>(0).KeepAlive();
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
}
