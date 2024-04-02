using SixtyThreeBits.Core.Infrastructure.Repositories;

namespace SixtyThreeBits.Core.Infrastructure.Factories
{
    public class RepositoryFactory
    {
        #region Properties                
        readonly DbContextFactory _connectionFactory;
        #endregion

        #region Constructors
        public RepositoryFactory(string dbConnectionString)
        {
            _connectionFactory = new DbContextFactory(dbConnectionString);
        }
        #endregion

        #region Methods
        public BlogPostsRepository GetBlogRepository()
        {
            return new BlogPostsRepository(_connectionFactory);
        }

        public BrandsRepository GetBrandsRepository()
        {
            return new BrandsRepository(_connectionFactory);
        }

        public CountriesRepository GetCountriesRepository()
        {
            return new CountriesRepository(_connectionFactory);
        }

        public DictionariesRepository GetDictionariesRepository()
        {
            return new DictionariesRepository(_connectionFactory);
        }

        public EmailTemplatesRepository GetEmailTemplatesRepository()
        {
            return new EmailTemplatesRepository(_connectionFactory);
        }

        public MenuFooterRepository GetMenuFooterRepository()
        {
            return new MenuFooterRepository(_connectionFactory);
        }

        public MenuHeaderRepository GetMenuHeaderRepository()
        {
            return new MenuHeaderRepository(_connectionFactory);
        }

        public NewsRepository GetNewsRepository()
        {
            return new NewsRepository(_connectionFactory);
        }

        public PagesRepository GetPagesRepository()
        {
            return new PagesRepository(_connectionFactory);
        }        

        public PermissionsRepository GetPermissionsRepository()
        {
            return new PermissionsRepository(_connectionFactory);
        }

        public ProductsRepository GetProductsRepository()
        {
            return new ProductsRepository(_connectionFactory);
        }

        public RedirectsRepository GetRedirectsRepository()
        {
            return new RedirectsRepository(_connectionFactory);
        }

        public RolesRepository GetRolesRepository()
        {
            return new RolesRepository(_connectionFactory);
        }

        public SystemPropertiesRepository GetSystemPropertiesRepository()
        {
            return new SystemPropertiesRepository(_connectionFactory);
        }

        public TeamMembersRepository GetTeamMembersRepository()
        {
            return new TeamMembersRepository(_connectionFactory);
        }

        public UsersRepository GetUsersRepository()
        {
            return new UsersRepository(_connectionFactory);
        }
        #endregion        
    }
}
