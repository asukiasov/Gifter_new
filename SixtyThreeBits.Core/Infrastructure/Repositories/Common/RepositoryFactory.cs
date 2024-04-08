using SixtyThreeBits.Core.Infrastructure.Database;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class RepositoryFactory
    {
        #region Properties                
        readonly DbContextFactory _dbContextFactory;
        #endregion

        #region Constructors
        public RepositoryFactory(string dbConnectionString)
        {
            _dbContextFactory = new DbContextFactory(dbConnectionString);
        }
        #endregion

        #region Methods
        public BlogPostsRepository GetBlogRepository()
        {
            return new BlogPostsRepository(_dbContextFactory);
        }

        public BrandsRepository GetBrandsRepository()
        {
            return new BrandsRepository(_dbContextFactory);
        }

        public CountriesRepository GetCountriesRepository()
        {
            return new CountriesRepository(_dbContextFactory);
        }

        public DictionariesRepository GetDictionariesRepository()
        {
            return new DictionariesRepository(_dbContextFactory);
        }

        public EmailTemplatesRepository GetEmailTemplatesRepository()
        {
            return new EmailTemplatesRepository(_dbContextFactory);
        }

        public MenuFooterRepository GetMenuFooterRepository()
        {
            return new MenuFooterRepository(_dbContextFactory);
        }

        public MenuHeaderRepository GetMenuHeaderRepository()
        {
            return new MenuHeaderRepository(_dbContextFactory);
        }

        public NewsRepository GetNewsRepository()
        {
            return new NewsRepository(_dbContextFactory);
        }

        public PagesRepository GetPagesRepository()
        {
            return new PagesRepository(_dbContextFactory);
        }

        public PermissionsRepository GetPermissionsRepository()
        {
            return new PermissionsRepository(_dbContextFactory);
        }

        public ProductsRepository GetProductsRepository()
        {
            return new ProductsRepository(_dbContextFactory);
        }

        public RedirectsRepository GetRedirectsRepository()
        {
            return new RedirectsRepository(_dbContextFactory);
        }

        public RolesRepository GetRolesRepository()
        {
            return new RolesRepository(_dbContextFactory);
        }

        public SystemPropertiesRepository GetSystemPropertiesRepository()
        {
            return new SystemPropertiesRepository(_dbContextFactory);
        }

        public TeamMembersRepository GetTeamMembersRepository()
        {
            return new TeamMembersRepository(_dbContextFactory);
        }

        public UsersRepository GetUsersRepository()
        {
            return new UsersRepository(_dbContextFactory);
        }
        #endregion        
    }
}
