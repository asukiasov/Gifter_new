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
        public DBCommandsDataContext GetDBCommandsDataContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DBCommandsDataContext>();
            optionsBuilder.UseSqlServer(_dbCommandsConnectionString);
            return new DBCommandsDataContext(optionsBuilder.Options);
        }

        public DBQueriesDataContext GetDBQueriesDataContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DBQueriesDataContext>();
            optionsBuilder.UseSqlServer(_dbQueriesConnectionString);
            return new DBQueriesDataContext(optionsBuilder.Options);
        }
        #endregion        
    }
}
