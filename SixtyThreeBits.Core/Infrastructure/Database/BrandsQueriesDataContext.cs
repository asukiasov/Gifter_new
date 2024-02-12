using SixtyThreeBits.Core.Infrastructure.Database.Core;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBQueriesDataContext
    {
        #region BrandsGetSingleByID
        public async Task<string> BrandsGetSingleByID(int? brandID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                databaseObjectName: nameof(BrandsGetSingleByID),
                itemType: typeof(ScalarFunctionResultEntity<string>),
                sqlParameters:
                [
                    brandID.ToSqlParameter(nameof(brandID), SqlDbType.Int)
                ]
             );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion

        #region BrandsList
        public record BrandsListEntity
        (            
            int? BrandID,
            string BrandName,
            string BrandNameEng,
            string BrandImageFilename,
            DateTime? BrandDateCreated
        );
        public IQueryable<BrandsListEntity> BrandsList()
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
                databaseObjectName: nameof(BrandsList),
                itemType: typeof(BrandsListEntity)
            );
            var result = sqb.ExecuteQuery<BrandsListEntity>();
            return result;
        }
        #endregion
    }
}