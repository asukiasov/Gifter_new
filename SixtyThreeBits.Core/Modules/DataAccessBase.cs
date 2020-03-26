using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Libraries;

namespace SixtyThreeBits.Core.Modules
{    
    public class DataAccessBase : SixtyThreeBitsDataObject
    {
        #region Properties
        public ConnectionFactory ConnectionFactory { get; set; }
        #endregion

        #region Constructors
        public DataAccessBase(ConnectionFactory ConnectionFactory)
        {
            this.ConnectionFactory = ConnectionFactory;
        }
        #endregion
    }
}
