namespace PowerScheduler.Actors;

public interface IActor<TPrimaryKey>
{
    TPrimaryKey ActorId { get; }

    Task OnActivateAsync(CancellationToken cancellationToken);
}
