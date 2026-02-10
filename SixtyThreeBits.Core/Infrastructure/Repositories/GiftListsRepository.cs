using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.Database;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class GiftListsRepository : RepositoryBase
    {
        #region Constructors
        public GiftListsRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {
        }
        #endregion

        #region Methods
        public async Task<List<GiftListsListDTO>> GiftListsList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(GiftListsList)}()",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(GiftListsList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<GiftListsListDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.GiftListDateCreated);
                        var result = await resultQueryable.ToListAsync();

                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<List<GiftListsListDTO>> GiftListsListByUserID(int userID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(GiftListsListByUserID)}({nameof(userID)} = {userID})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(GiftListsListByUserID),
                            sqlParameters:
                            [
                                userID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<GiftListsListDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.GiftListDateCreated);
                        var result = await resultQueryable.ToListAsync();

                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<GiftListsListDTO> GiftListsGetSingleByID(int giftListID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(GiftListsGetSingleByID)}({nameof(giftListID)} = {giftListID})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(GiftListsGetSingleByID),
                            sqlParameters:
                            [
                                giftListID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson?.DeserializeJsonTo<GiftListsListDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> GiftListsIUD(Enums.DatabaseActions databaseAction, int? giftListID, GiftListIudDTO giftList)
        {
            var giftListJson = giftList.ToJson();

            giftListID = await TryToReturnAsyncTask(
                logString: $"{nameof(GiftListsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(giftListID)} = {giftListID}, {nameof(giftList)} = {giftListJson})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(GiftListsIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(SqlDbType.TinyInt),
                                giftListID.ToSqlParameterOutput(SqlDbType.Int),
                                giftListJson.ToSqlParameter(SqlDbType.NVarChar)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        giftListID = sqb.GetNextOutputParameterValue<int?>();
                        return giftListID;
                    }
                }
            );
            return giftListID;
        }
        #endregion
    }
}
