using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;

namespace SixtyThreeBits.Core.Modules
{
    public class DataAccessFactory
    {
        #region Properties
        public RolesDataAccess Roles { get; set; }
        public PermissionsDataAccess Permissions { get; set; }
        public UsersDataAccess Users { get; set; }
        public DictionariesDataAccess Dictionaries { get; set; }
        #endregion

        #region Constructors
        public DataAccessFactory(string DBConnectionString)
        {
            var ConnectionFactory = new ConnectionFactory(DBConnectionString);
            Roles = new RolesDataAccess(ConnectionFactory);
            Permissions = new PermissionsDataAccess(ConnectionFactory);
            Users = new UsersDataAccess(ConnectionFactory);
            Dictionaries = new DictionariesDataAccess(ConnectionFactory);
        } 
        #endregion
    }

    public class ConnectionFactory
    {
        readonly string DBConnectionString;

        public ConnectionFactory(string DBConnectionString)
        {
            this.DBConnectionString = DBConnectionString;
        }
        public DBCoreDataContext GetDBCoreDataContext()
        {
            var OptionsBuilder = new DbContextOptionsBuilder<DBCoreDataContext>();
            OptionsBuilder.UseSqlServer(DBConnectionString);
            return new DBCoreDataContext(OptionsBuilder.Options);
        }
    }
}
