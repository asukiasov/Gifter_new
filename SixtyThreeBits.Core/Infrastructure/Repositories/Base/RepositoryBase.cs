using AutoMapper;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Libraries;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.Base
{
    public class RepositoryBase : SixtyThreeBitsDataObjectBase
    {
        #region Properties
        protected readonly ConnectionFactory _connectionFactory;
        protected IMapper _mapper;
        #endregion

        #region Constructors
        public RepositoryBase(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        #endregion
    }
}