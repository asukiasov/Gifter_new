using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Libraries;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.Base
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