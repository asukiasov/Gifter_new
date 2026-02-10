using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Infrastructure.Repositories;

namespace SixtyThreeBits.Core.Factories
{
    public class RepositoryFactory
    {
        #region Properties                
        readonly DbContextFactory _dbContextFactory;
        readonly ILogger _logger;
        #endregion

        #region Constructors
        public RepositoryFactory(string dbConnectionString, ILogger logger = null)
        {
            _dbContextFactory = new DbContextFactory(dbConnectionString);
            _logger = logger;
        }
        #endregion

        #region Methods
        public CountriesRepository CreateCountriesRepository()
        {
            return new CountriesRepository(_dbContextFactory, _logger);
        }

        public DictionariesRepository CreateDictionariesRepository()
        {
            return new DictionariesRepository(_dbContextFactory, _logger);
        }

        public EmailTemplatesRepository CreateEmailTemplatesRepository()
        {
            return new EmailTemplatesRepository(_dbContextFactory, _logger);
        }

        public FollowersRepository CreateFollowersRepository()
        {
            return new FollowersRepository(_dbContextFactory, _logger);
        }

        public MenuFooterRepository CreateMenuFooterRepository()
        {
            return new MenuFooterRepository(_dbContextFactory, _logger);
        }

        public MenuHeaderRepository CreateMenuHeaderRepository()
        {
            return new MenuHeaderRepository(_dbContextFactory, _logger);
        }

        public GiftListsRepository CreateGiftListsRepository()
        {
            return new GiftListsRepository(_dbContextFactory, _logger);
        }

        public GiftsRepository CreateGiftsRepository()
        {
            return new GiftsRepository(_dbContextFactory, _logger);
        }

        public OrdersRepository CreateOrdersRepository()
        {
            return new OrdersRepository(_dbContextFactory, _logger);
        }

        public PagesRepository CreatePagesRepository()
        {
            return new PagesRepository(_dbContextFactory, _logger);
        }

        public PermissionsRepository CreatePermissionsRepository()
        {
            return new PermissionsRepository(_dbContextFactory, _logger);
        }

        public RedirectsRepository CreateRedirectsRepository()
        {
            return new RedirectsRepository(_dbContextFactory, _logger);
        }

        public RolesRepository CreateRolesRepository()
        {
            return new RolesRepository(_dbContextFactory, _logger);
        }

        public SystemPropertiesRepository CreateSystemPropertiesRepository()
        {
            return new SystemPropertiesRepository(_dbContextFactory, _logger);
        }

        public UsersRepository CreateUsersRepository()
        {
            return new UsersRepository(_dbContextFactory, _logger);
        }
        #endregion        
    }
}
