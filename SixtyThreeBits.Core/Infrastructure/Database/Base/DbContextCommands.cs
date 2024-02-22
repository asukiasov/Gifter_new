using Microsoft.EntityFrameworkCore;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextCommands : DbContext
    {
        #region Constructors
        public DbContextCommands(DbContextOptions<DbContextCommands> options) : base(options)
        {
        }
        #endregion        
    }
}