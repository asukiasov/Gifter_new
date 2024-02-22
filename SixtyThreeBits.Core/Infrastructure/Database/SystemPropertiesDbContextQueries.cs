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
                databaseObjectType: DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                databaseObjectName: nameof(SystemPropertiesGet),
                itemType: typeof(ScalarFunctionResultEntity<string>)
            );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion
    }
}