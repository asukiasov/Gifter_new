using Microsoft.EntityFrameworkCore;

namespace SixtyThreeBits.Core.Factories
{
    public class DbContextFactory
    {
        #region Properties
        string _dbConnectionString;
        #endregion

        #region Constructors
        public DbContextFactory(string dbConnectionString)
        {
            _dbConnectionString = dbConnectionString;
        }
        #endregion

        #region Methods
        public DbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContext>();
            optionsBuilder.UseSqlServer(_dbConnectionString);
            return new DbContext(optionsBuilder.Options);
        }
        #endregion        
    }
}
