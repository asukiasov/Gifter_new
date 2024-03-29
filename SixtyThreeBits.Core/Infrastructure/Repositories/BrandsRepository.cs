using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
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
        public BrandsRepository(DbContextFactory connectionFactory) : base(connectionFactory)
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

        public async Task<int?> BrandsIUD(Enums.DatabaseActions databaseAction, int? brandID = null, string brandName = null, string brandNameEng = null, string brandImageFilename = null)
        {
            brandID = await TryToReturnAsyncTask(
                logString: $"{nameof(BrandsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(brandID)} = {brandID}, {nameof(brandName)} = {brandName}, {nameof(brandNameEng)} = {brandNameEng})", 
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
                                 brandName.ToSqlParameter(nameof(brandName),SqlDbType.NVarChar),
                                 brandNameEng.ToSqlParameter(nameof(brandNameEng),SqlDbType.NVarChar),
                                 brandImageFilename.ToSqlParameter(nameof(brandImageFilename),SqlDbType.NVarChar)
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

        public async Task<List<BrandDTO>> BrandsList()
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

                        var resultQueryable = sqb.ExecuteTableValuedFunction<BrandDTO>();
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