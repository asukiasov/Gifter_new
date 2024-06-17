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
        public BlogPostsRepository CreateBlogRepository()
        {
            return new BlogPostsRepository(_dbContextFactory);
        }

        public BrandsRepository CreateBrandsRepository()
        {
            return new BrandsRepository(_dbContextFactory);
        }

        public CountriesRepository CreateCountriesRepository()
        {
            return new CountriesRepository(_dbContextFactory);
        }

        public DictionariesRepository CreateDictionariesRepository()
        {
            return new DictionariesRepository(_dbContextFactory);
        }

        public EmailTemplatesRepository CreateEmailTemplatesRepository()
        {
            return new EmailTemplatesRepository(_dbContextFactory);
        }

        public MenuFooterRepository CreateMenuFooterRepository()
        {
            return new MenuFooterRepository(_dbContextFactory);
        }

        public MenuHeaderRepository CreateMenuHeaderRepository()
        {
            return new MenuHeaderRepository(_dbContextFactory);
        }

        public NewsRepository CreateNewsRepository()
        {
            return new NewsRepository(_dbContextFactory);
        }

        public PagesRepository CreatePagesRepository()
        {
            return new PagesRepository(_dbContextFactory);
        }

        public PermissionsRepository CreatePermissionsRepository()
        {
            return new PermissionsRepository(_dbContextFactory);
        }

        public ProductsRepository CreateProductsRepository()
        {
            return new ProductsRepository(_dbContextFactory);
        }

        public RedirectsRepository CreateRedirectsRepository()
        {
            return new RedirectsRepository(_dbContextFactory);
        }

        public RolesRepository CreateRolesRepository()
        {
            return new RolesRepository(_dbContextFactory);
        }

        public SystemPropertiesRepository CreateSystemPropertiesRepository()
        {
            return new SystemPropertiesRepository(_dbContextFactory);
        }

        public TeamMembersRepository CreateTeamMembersRepository()
        {
            return new TeamMembersRepository(_dbContextFactory);
        }

        public UsersRepository CreateUsersRepository()
        {
            return new UsersRepository(_dbContextFactory);
        }
        #endregion        
    }
}
