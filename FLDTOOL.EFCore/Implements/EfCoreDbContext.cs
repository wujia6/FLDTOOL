using System.Runtime.Loader;
using FLDTOOL.EFCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FLDTOOL.EFCore.Implements
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="options"></param>
    public class EfCoreDbContext<TDbContext>(EfCoreDbContextOptions<TDbContext> options) : DbContext(options.Options) where TDbContext : DbContext
    {
        private readonly string _dllPath = options.DllPath;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(_dllPath);
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }

        public async Task<int> SaveChangesAsync() => await base.SaveChangesAsync();

        public async Task BeginTrans() => await base.Database.BeginTransactionAsync();

        public async Task CommitTrans() => await base.Database.CommitTransactionAsync();

        public async Task RollBackTrans() => await base.Database.RollbackTransactionAsync();
    }
}
