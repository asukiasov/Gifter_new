using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Libraries;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class RepositoryBase : SixtyThreeBitsDataObjectBase
    {
        #region Properties
        protected readonly DbContextFactory _dbContextFactory;
        #endregion

        #region Constructors
        public RepositoryBase(DbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        #endregion
    }
}