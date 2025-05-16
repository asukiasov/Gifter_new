using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Libraries;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class RepositoryBase : SixtyThreeBitsDataObjectBase
    {
        #region Properties
        protected readonly DbContextFactory _dbContextFactory;
        #endregion

        #region Constructors
        public RepositoryBase(DbContextFactory dbContextFactory, ILogger logger) : base(logger)
        {
            _dbContextFactory = dbContextFactory;
        }
        #endregion
    }
}