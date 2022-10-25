using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Orleans;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Timing;

namespace PowerScheduler.Actors;

public abstract class ActorBase : ActorBase<Guid>
{

}

public abstract class ActorBase<TPrimaryKey> : Grain, IActor<TPrimaryKey>
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; private set; }

    protected IClock Clock => LazyServiceProvider.LazyGetRequiredService<IClock>();

    protected IGuidGenerator GuidGenerator => LazyServiceProvider.LazyGetRequiredService<IGuidGenerator>();

    protected IGrainFactory ActorClient => LazyServiceProvider.LazyGetRequiredService<IGrainFactory>();

    protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

    protected ILogger Logger => LazyServiceProvider.LazyGetService<ILogger>((IServiceProvider provider) => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance);

    public TPrimaryKey ActorId { get; private set; }

    protected Type ActorType => GetType();

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var type = typeof(TPrimaryKey);
        if (type == typeof(long) && this.GetPrimaryKeyLong() is TPrimaryKey longKey)
        {
            this.ActorId = longKey;
        }
        else if (type == typeof(string) && this.GetPrimaryKeyString() is TPrimaryKey stringKey)
        {
            this.ActorId = stringKey;
        }
        else if (type == typeof(Guid) && this.GetPrimaryKey() is TPrimaryKey guidKey)
        {
            this.ActorId = guidKey;
        }
        else
        {
            throw new ArgumentOutOfRangeException(typeof(TPrimaryKey).FullName);
        }

        LazyServiceProvider = this.ServiceProvider.GetRequiredService<IAbpLazyServiceProvider>();

        return base.OnActivateAsync(cancellationToken);
    }
}
