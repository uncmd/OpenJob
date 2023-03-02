using Volo.Abp.Domain.Entities;

namespace Microsoft.EntityFrameworkCore;

public static class DeleteExtensions
{
    public static async Task DeleteAsync<TEntity>(
        this DbContext db,
        TEntity entity,
        bool throwOnNotFound = false)
        where TEntity : IEntity
    {
        try
        {
            if (entity is null) return;
            var track = db.Attach(entity);
            track.State = EntityState.Deleted;
            await db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // if entity doesn't exist in database, 
            // this will throw 
            if (throwOnNotFound)
                throw;
        }
    }
}
