﻿using Microsoft.Extensions.Logging;
using OpenJob.Server.Actors;
using Orleans.Runtime;
using System.Reflection;

namespace OpenJob.Server;

public class SchedulerStartup : IStartupTask
{
    private readonly IGrainFactory _grainFactory;
    private readonly ILogger<SchedulerStartup> _logger;

    public SchedulerStartup(
        IGrainFactory grainFactory,
        ILogger<SchedulerStartup> logger)
    {
        _grainFactory = grainFactory;
        _logger = logger;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        var actor = _grainFactory.GetGrain<ISchedulerReminder>(0);

        var orleansVersion = GetOrleansVersion();
        var hostVersion = GetHostVersion();

        _logger.LogInformation("OrleansVersion: {OrleansVersion}, HostVersion: {HostVersion}", orleansVersion, hostVersion);

        await actor.SetVersion(orleansVersion, hostVersion);

        _logger.LogInformation("OpenJob starting ... ");

        await actor.Active();
    }

    private static string GetOrleansVersion()
    {
        var assembly = typeof(SiloAddress).GetTypeInfo().Assembly;
        return string.Format("{0} ({1})",
            assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion,
            assembly.GetName().Version.ToString());
    }

    private static string GetHostVersion()
    {
        try
        {
            var assembly = Assembly.GetEntryAssembly();

            if (assembly != null)
            {
                return assembly.GetName().Version.ToString();
            }
        }
        catch
        {
            /* NOOP */
        }

        return "1.0.0.0";
    }
}
