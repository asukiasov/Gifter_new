using Microsoft.EntityFrameworkCore;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBCommandsDataContext : DbContext
    {
        #region Constructors
        public DBCommandsDataContext(DbContextOptions<DBCommandsDataContext> options) : base(options)
        {
        }
        #endregion        
    }
}