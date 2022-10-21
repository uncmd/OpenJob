using Orleans;

namespace PowerScheduler.Actors;

public static class ActorServiceProviderExtentions
{
    public static TGrainInterface GetActor<TGrainInterface>(this IServiceProvider service, Guid id)
        where TGrainInterface : IGrainWithGuidKey
    {
        var actorFactory = service.GetRequiredService<IGrainFactory>();
        return actorFactory.GetGrain<TGrainInterface>(id);
    }

    public static TGrainInterface GetActor<TGrainInterface>(this IServiceProvider service, long id)
        where TGrainInterface : IGrainWithIntegerKey
    {
        var actorFactory = service.GetRequiredService<IGrainFactory>();
        return actorFactory.GetGrain<TGrainInterface>(id);
    }

    public static TGrainInterface GetActor<TGrainInterface>(this IServiceProvider service, string id)
        where TGrainInterface : IGrainWithStringKey
    {
        var actorFactory = service.GetRequiredService<IGrainFactory>();
        return actorFactory.GetGrain<TGrainInterface>(id);
    }

    public static TGrainInterface GetActor<TGrainInterface>(this IServiceProvider service, Guid id, string keyExtension)
        where TGrainInterface : IGrainWithGuidCompoundKey
    {
        var actorFactory = service.GetRequiredService<IGrainFactory>();
        return actorFactory.GetGrain<TGrainInterface>(id, keyExtension);
    }

    public static TGrainInterface GetActor<TGrainInterface>(this IServiceProvider service, long id, string keyExtension)
        where TGrainInterface : IGrainWithIntegerCompoundKey
    {
        var actorFactory = service.GetRequiredService<IGrainFactory>();
        return actorFactory.GetGrain<TGrainInterface>(id, keyExtension);
    }
}
