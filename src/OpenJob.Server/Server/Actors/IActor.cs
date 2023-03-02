namespace OpenJob.Server.Actors;

public interface IActor<TPrimaryKey>
{
    TPrimaryKey ActorId { get; }

    Task OnActivateAsync(CancellationToken cancellationToken);
}
