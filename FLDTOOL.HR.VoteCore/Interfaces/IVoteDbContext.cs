using FLDTOOL.EFCore.Interfaces;
using FLDTOOL.HR.VoteCore.Implements;

namespace FLDTOOL.HR.VoteCore.Interfaces
{
    public interface IVoteDbContext : IEfCoreDbContext<VoteDbContext>, IUnitOfWork
    {

    }
}
