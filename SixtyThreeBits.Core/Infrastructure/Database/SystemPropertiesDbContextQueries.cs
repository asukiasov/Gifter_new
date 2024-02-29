using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextQueries
    {
        #region SystemPropertiesGet        
        public async Task<string> SystemPropertiesGet()
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(SystemPropertiesGet)
            );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion
    }
}