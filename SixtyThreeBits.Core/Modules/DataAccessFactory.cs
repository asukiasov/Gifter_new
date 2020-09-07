using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;

namespace SixtyThreeBits.Core.Modules
{
    public class DataAccessFactory
    {
        #region Properties        
        public BlogsDataAccess Blog{ get; set; }
        public DictionariesDataAccess Dictionaries { get; set; }
        public PagesDataAccess Pages { get; set; }
        public PermissionsDataAccess Permissions { get; set; }
        public RolesDataAccess Roles { get; set; }
        public SystemPropertiesAccess SystemProperties { get; set; }
        public UsersDataAccess Users { get; set; }
        #endregion

        #region Constructors
        public DataAccessFactory(AppSettingsCollection AppSettings)
        {
            var ConnectionFactory = new ConnectionFactory(AppSettings.DBConnectionStrings.DBConnectionString);
            Blog = new BlogsDataAccess(ConnectionFactory);
            Dictionaries = new DictionariesDataAccess(ConnectionFactory);
            Pages = new PagesDataAccess(ConnectionFactory, AppSettings);
            Permissions = new PermissionsDataAccess(ConnectionFactory);
            Roles = new RolesDataAccess(ConnectionFactory);
            SystemProperties = new SystemPropertiesAccess(ConnectionFactory);
            Users = new UsersDataAccess(ConnectionFactory);
        } 
        #endregion
    }

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
