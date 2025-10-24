using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FLDTOOL.EFCore.Interfaces
{
    public interface IEfCoreDbContext<TDbContext> where TDbContext : DbContext
    {
        EntityEntry<T> Entry<T>(T entity) where T : class;
        DbSet<T> Set<T>() where T : class;
    }
}
