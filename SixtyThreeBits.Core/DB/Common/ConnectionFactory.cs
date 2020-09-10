using Microsoft.EntityFrameworkCore;

namespace SixtyThreeBits.Core.DB
{
    public class ConnectionFactory
    {
        #region Properties
        readonly string DBConnectionString;        
        #endregion

        #region Constructors
        public ConnectionFactory(string DBConnectionString)
        {
            this.DBConnectionString = DBConnectionString;
        }
        #endregion

        #region Methods
        public DBCoreDataContext GetDBCoreDataContext()
        {
            var OptionsBuilder = new DbContextOptionsBuilder<DBCoreDataContext>();
            OptionsBuilder.UseSqlServer(DBConnectionString);
            return new DBCoreDataContext(OptionsBuilder.Options);
        }
        #endregion
    }
}
