using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Mst.DomainDrivenDesign.EfCore;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IAggregateRoot
{
    protected DbSet<TEntity> DbSet { get; }
    protected DbContext DatabaseContext { get; }

    public Repository(DbContext databaseContext)
    {
        DatabaseContext = databaseContext ?? 
            throw new ArgumentNullException(paramName: nameof(databaseContext));
        DbSet = DatabaseContext.Set<TEntity>();
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(paramName: nameof(entity));

        await DbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        if (entities == null)
            throw new ArgumentNullException(paramName: nameof(entities));

        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(paramName: nameof(entity));

        await Task.Run(() => DbSet.Remove(entity), cancellationToken);
    }

    public async Task<bool> RemoveByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        TEntity entity = await FindAsync(id, cancellationToken);
        if (entity == null) return false;

        await RemoveAsync(entity, cancellationToken);
        return true;
    }

    public async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        if (entities == null)
            throw new ArgumentNullException(paramName: nameof(entities));

        foreach (var entity in entities)
        {
            await RemoveAsync(entity, cancellationToken);
        }
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Task.Run(() =>
        {
            var attachedEntity = DatabaseContext.Attach(entity);
            if (attachedEntity.State != EntityState.Modified)
            {
                attachedEntity.State = EntityState.Modified;
            }
        }, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<TEntity> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public Task<IEnumerable<TEntity>> GetSomeAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}