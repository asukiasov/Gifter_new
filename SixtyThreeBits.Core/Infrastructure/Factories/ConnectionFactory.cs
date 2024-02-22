using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.Infrastructure.Database;

namespace SixtyThreeBits.Core.Infrastructure.Factories
{
    public class ConnectionFactory
    {
        #region Properties
        string _dbCommandsConnectionString;
        string _dbQueriesConnectionString;
        #endregion

        #region Constructors
        public ConnectionFactory(string dbCommandsConnectionString, string dbQueriesConnectionString)
        {
            _dbCommandsConnectionString = dbCommandsConnectionString;
            _dbQueriesConnectionString = dbQueriesConnectionString;
        }
        #endregion

        #region Methods
        public DbContextCommands GetDbContextCommands()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContextCommands>();
            optionsBuilder.UseSqlServer(_dbCommandsConnectionString);
            return new DbContextCommands(optionsBuilder.Options);
        }

        public DbContextQueries GetDbContextQueries()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContextQueries>();
            optionsBuilder.UseSqlServer(_dbQueriesConnectionString);
            return new DbContextQueries(optionsBuilder.Options);
        }
        #endregion        
    }
}
