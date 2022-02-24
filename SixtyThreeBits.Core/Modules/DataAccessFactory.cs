using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;

namespace SixtyThreeBits.Core.Modules
{
    public class DataAccessFactory
    {
        #region Properties        
        public BlogDataAccess Blog { get; set; }
        public BrandsDataAccess Brands { get; set; }
        public CategoriesDataAccess Categories { get; set; }
        public DictionariesDataAccess Dictionaries { get; set; }
        public NewsDataAccess News { get; set; }
        public PagesDataAccess Pages { get; set; }
        public PartnersDataAccess Partners { get; set; }
        public PermissionsDataAccess Permissions { get; set; }
        public ProductsDataAccess Products { get; set; }
        public ProjectsDataAccess Projects { get; set; }
        public RolesDataAccess Roles { get; set; }
        public SystemPropertiesAccess SystemProperties { get; set; }
        public TeamMembersDataAccess TeamMembers { get; set; }
        public UsersDataAccess Users { get; set; }
        #endregion

        #region Constructors
        public DataAccessFactory(AppSettingsCollection AppSettings, UtilityCollection Utilities)
        {
            var ConnectionFactory = new ConnectionFactory(AppSettings.DBConnectionStrings.DBConnectionString);
            Blog = new BlogDataAccess(ConnectionFactory);
            Brands = new BrandsDataAccess(ConnectionFactory);
            Categories = new CategoriesDataAccess(ConnectionFactory, Utilities);
            Dictionaries = new DictionariesDataAccess(ConnectionFactory);
            News = new NewsDataAccess(ConnectionFactory);
            Pages = new PagesDataAccess(ConnectionFactory, AppSettings);
            Partners = new PartnersDataAccess(ConnectionFactory);
            Products = new ProductsDataAccess(ConnectionFactory);
            Permissions = new PermissionsDataAccess(ConnectionFactory);
            Projects = new ProjectsDataAccess(ConnectionFactory, Utilities);
            Roles = new RolesDataAccess(ConnectionFactory);
            TeamMembers = new TeamMembersDataAccess(ConnectionFactory);
            SystemProperties = new SystemPropertiesAccess(ConnectionFactory);
            Users = new UsersDataAccess(ConnectionFactory);
        } 
        #endregion
    }    
}
