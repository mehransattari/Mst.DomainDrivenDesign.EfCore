using Microsoft.EntityFrameworkCore;

namespace Mst.DomainDrivenDesign.EfCore;

public abstract class UnitOfWork<TDbContext> : IUnitOfWork where TDbContext : DbContext
{
    protected TDbContext DatabaseContext { get; }

    public bool IsDisposed { get; protected set; }

    public UnitOfWork(TDbContext databaseContext)
    {
        DatabaseContext = databaseContext ?? 
            throw new ArgumentNullException(nameof(databaseContext));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed) return;

        if (disposing)
        {
            DatabaseContext?.Dispose();
        }

        IsDisposed = true;
    }

    public async Task<int> SaveAsync()
    {
        return await DatabaseContext.SaveChangesAsync();
    }

    ~UnitOfWork()
    {
        Dispose(false);
    }
}