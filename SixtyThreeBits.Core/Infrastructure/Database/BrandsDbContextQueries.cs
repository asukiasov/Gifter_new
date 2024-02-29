using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextQueries
    {
        #region BrandsGetSingleByID
        public async Task<string> BrandsGetSingleByID(int? brandID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(BrandsGetSingleByID),
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
                databaseObjectName: nameof(BrandsList)
            );
            var result = sqb.ExecuteQuery<BrandsListEntity>();
            return result;
        }
        #endregion
    }
}