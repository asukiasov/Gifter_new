using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;

namespace SixtyThreeBits.Core.Modules
{
    public class DataAccessFactory
    {
        #region Properties        
        public BlogsDataAccess Blog { get; set; }
        public DictionariesDataAccess Dictionaries { get; set; }
        public NewsDataAccess News { get; set; }
        public PagesDataAccess Pages { get; set; }
        public PermissionsDataAccess Permissions { get; set; }
        public ProjectsDataAccess Projects { get; set; }
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
            News = new NewsDataAccess(ConnectionFactory);
            Pages = new PagesDataAccess(ConnectionFactory, AppSettings);
            Permissions = new PermissionsDataAccess(ConnectionFactory);
            Projects = new ProjectsDataAccess(ConnectionFactory);
            Roles = new RolesDataAccess(ConnectionFactory);
            SystemProperties = new SystemPropertiesAccess(ConnectionFactory);
            Users = new UsersDataAccess(ConnectionFactory);
        } 
        #endregion
    }    
}
