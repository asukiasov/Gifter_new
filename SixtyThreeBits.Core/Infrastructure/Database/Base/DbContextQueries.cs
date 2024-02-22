using Microsoft.EntityFrameworkCore;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextQueries : DbContext
    {
        #region Constructors
        public DbContextQueries(DbContextOptions<DbContextQueries> options) : base(options)
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