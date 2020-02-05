using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Libraries;

namespace SixtyThreeBits.Core.Modules
{    
    public class DataAccessBase : SixtyThreeBitsDataObject
    {
        #region Properties
        public DBCoreDataContext db { get; set; }
        #endregion

        #region Constructors
        public DataAccessBase(DBCoreDataContext db)
        {
            this.db = db;
        }
        #endregion
    }
}
