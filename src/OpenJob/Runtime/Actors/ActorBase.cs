using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Timing;

namespace OpenJob.Runtime.Actors;

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

    protected ILogger Logger => LazyServiceProvider.LazyGetService<ILogger>((provider) => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance);

    public TPrimaryKey ActorId { get; private set; }

    protected Type ActorType => GetType();

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var type = typeof(TPrimaryKey);
        if (type == typeof(long) && this.GetPrimaryKeyLong() is TPrimaryKey longKey)
        {
            ActorId = longKey;
        }
        else if (type == typeof(string) && this.GetPrimaryKeyString() is TPrimaryKey stringKey)
        {
            ActorId = stringKey;
        }
        else if (type == typeof(Guid) && this.GetPrimaryKey() is TPrimaryKey guidKey)
        {
            ActorId = guidKey;
        }
        else
        {
            throw new ArgumentOutOfRangeException(typeof(TPrimaryKey).FullName);
        }

        LazyServiceProvider = ServiceProvider.GetRequiredService<IAbpLazyServiceProvider>();

        return base.OnActivateAsync(cancellationToken);
    }
}
