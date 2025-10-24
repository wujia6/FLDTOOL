using FLDTOOL.EFCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FLDTOOL.HR.VoteCore.Interfaces
{
    public interface IVoteRepository<TDbContext, TEntity> : IEfCoreRepository<TDbContext, TEntity>
        where TDbContext: DbContext 
        where TEntity: class
    {

    }
}
