using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class BrandsRepository : RepositoryBase
    {
        #region Constructors
        public BrandsRepository(DbContextFactory dbContextFactory) : base(dbContextFactory)
        {
            
        }
        #endregion

        #region Methods
        public async Task<BrandDTO> BrandsGetSingleByID(int? brandID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(BrandsGetSingleByID)}({nameof(brandID)} = {brandID})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(BrandsGetSingleByID),
                            sqlParameters:
                            [
                                brandID.ToSqlParameter(nameof(brandID), SqlDbType.Int)
                            ]
                        );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<BrandDTO>();

                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> BrandsIUD(Enums.DatabaseActions databaseAction, int? brandID, BrandIudDTO brand)
        {
            var brandJson = brand.ToJson();

            brandID = await TryToReturnAsyncTask(
                logString: $"{nameof(BrandsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(brandID)} = {brandID}, {nameof(brand)} = {brandJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(BrandsIUD),
                            sqlParameters:
                            [
                                 databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                                 brandID.ToSqlOutputParameter(nameof(brandID),SqlDbType.Int),
                                 brandJson.ToSqlParameter(nameof(brandJson),SqlDbType.NVarChar)
                            ]
                        );

                        var DBResult = await sqb.ExecuteStoredProcedure();
                        brandID = sqb.GetNextOutputParameterValue<int?>();
                        return brandID;
                    }
                }
            );
            return brandID;
        }

        public async Task<List<BrandListDTO>> BrandsList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(BrandsList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(BrandsList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<BrandListDTO>();
                        var result = await resultQueryable
                            .OrderByDescending(item => item.BrandDateCreated)
                            .ToListAsync();
                        return result;
                    }
                }
            );
            return result;
        }
        #endregion
    }        
}