using AutoMapper;
using SixtyThreeBits.Core.Infrastructure.Database.Core;

namespace SixtyThreeBits.Core.Infrastructure.Base
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
