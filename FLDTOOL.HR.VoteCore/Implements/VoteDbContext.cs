using FLDTOOL.EFCore;
using FLDTOOL.EFCore.Implements;
using FLDTOOL.HR.VoteCore.Interfaces;

namespace FLDTOOL.HR.VoteCore.Implements
{
    public class VoteDbContext(EfCoreDbContextOptions<VoteDbContext> options) : EfCoreDbContext<VoteDbContext>(options), IVoteDbContext
    {

    }
}
