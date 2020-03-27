using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;

namespace SixtyThreeBits.Core.Modules
{
    public class DataAccessFactory
    {
        #region Properties        
        public DictionariesDataAccess Dictionaries { get; set; }
        public PagesDataAccess Pages { get; set; }
        public PermissionsDataAccess Permissions { get; set; }
        public RolesDataAccess Roles { get; set; }
        public UsersDataAccess Users { get; set; }        
        #endregion

        #region Constructors
        public DataAccessFactory(AppSettingsCollection AppSettings)
        {
            var ConnectionFactory = new ConnectionFactory(AppSettings.DBConnectionStrings.DBConnectionString);
            Dictionaries = new DictionariesDataAccess(ConnectionFactory);
            Pages = new PagesDataAccess(ConnectionFactory, AppSettings);
            Permissions = new PermissionsDataAccess(ConnectionFactory);
            Roles = new RolesDataAccess(ConnectionFactory);            
            Users = new UsersDataAccess(ConnectionFactory);            
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
