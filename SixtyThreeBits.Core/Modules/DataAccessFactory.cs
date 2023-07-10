using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;

namespace SixtyThreeBits.Core.Modules
{
    public class DataAccessFactory
    {
        #region Properties        
        public BlogDataAccess Blog { get; private set; }
        public BrandsDataAccess Brands { get; private set; }
        public DictionariesDataAccess Dictionaries { get; private set; }
        public EmailTemplatesDataAccess EmailTemplates { get; private set; }
        public NewsDataAccess News { get; set; }
        public NotificationManagerDataAccess NotificationManager { get; private set; }
        public PagesDataAccess Pages { get; private set; }
        public PartnersDataAccess Partners { get; private set; }
        public PermissionsDataAccess Permissions { get; private set; }
        public ProductsDataAccess Products { get; private set; }
        public ProjectsDataAccess Projects { get; private set; }
        public RedirectsDataAccess Redirects { get; private set; }
        public RolesDataAccess Roles { get; private set; }
        public SystemPropertiesAccess SystemProperties { get; private set; }
        public TeamMembersDataAccess TeamMembers { get; private set; }
        public UsersDataAccess Users { get; private set; }
        #endregion

        #region Constructors
        public DataAccessFactory(AppSettingsCollection AppSettings, UtilityCollection Utilities)
        {
            var ConnectionFactory = new ConnectionFactory(AppSettings.DBConnectionStrings.DBConnectionString);
            Blog = new BlogDataAccess(ConnectionFactory);
            Brands = new BrandsDataAccess(ConnectionFactory);
            Dictionaries = new DictionariesDataAccess(ConnectionFactory);
            EmailTemplates = new EmailTemplatesDataAccess(ConnectionFactory);
            News = new NewsDataAccess(ConnectionFactory, AppSettings);
            NotificationManager = new NotificationManagerDataAccess(ConnectionFactory);
            Pages = new PagesDataAccess(ConnectionFactory, AppSettings);
            Partners = new PartnersDataAccess(ConnectionFactory);
            Products = new ProductsDataAccess(ConnectionFactory, Utilities);
            Permissions = new PermissionsDataAccess(ConnectionFactory);
            Projects = new ProjectsDataAccess(ConnectionFactory, Utilities);
            Redirects = new RedirectsDataAccess(ConnectionFactory);
            Roles = new RolesDataAccess(ConnectionFactory);
            TeamMembers = new TeamMembersDataAccess(ConnectionFactory);
            SystemProperties = new SystemPropertiesAccess(ConnectionFactory);
            Users = new UsersDataAccess(ConnectionFactory);
        } 
        #endregion
    }    
}
