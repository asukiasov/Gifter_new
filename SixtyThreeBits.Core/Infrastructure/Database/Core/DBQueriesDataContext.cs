using Microsoft.EntityFrameworkCore;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBQueriesDataContext : DbContext
    {
        #region Constructors
        public DBQueriesDataContext(DbContextOptions<DBQueriesDataContext> options) : base(options)
        {
        }
        #endregion

        #region Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
        }
        #endregion

        #region Nested Classes
        public record ScalarFunctionResultEntity<T>
        {
            #region Properties
            public T Value { get; init; }
            #endregion
        }
        #endregion
    }
}