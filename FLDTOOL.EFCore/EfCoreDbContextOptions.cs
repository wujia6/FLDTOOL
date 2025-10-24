using Microsoft.EntityFrameworkCore;

namespace FLDTOOL.EFCore
{
    public class EfCoreDbContextOptions<TDbContext>(DbContextOptions<TDbContext> options, string dllPath) where TDbContext : DbContext
    {
        public DbContextOptions<TDbContext> Options { get; } = options ?? throw new ArgumentNullException(nameof(options));
        public string DllPath { get; } = dllPath ?? throw new ArgumentNullException(nameof(dllPath));
    }
}
