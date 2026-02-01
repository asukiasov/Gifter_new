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
    public class GiftsRepository : RepositoryBase
    {
        #region Constructors
        public GiftsRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {
        }
        #endregion

        #region Methods
        public async Task<List<GiftsListDTO>> GiftsList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(GiftsList)}()",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(GiftsList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<GiftsListDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.GiftDateCreated);
                        var result = await resultQueryable.ToListAsync();

                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> GiftsIUD(Enums.DatabaseActions databaseAction, int? giftID, GiftIudDTO gift)
        {
            var giftJson = gift.ToJson();

            giftID = await TryToReturnAsyncTask(
                logString: $"{nameof(GiftsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(giftID)} = {giftID}, {nameof(gift)} = {giftJson})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(GiftsIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(SqlDbType.TinyInt),
                                giftID.ToSqlParameterOutput(SqlDbType.Int),
                                giftJson.ToSqlParameter(SqlDbType.NVarChar)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        giftID = sqb.GetNextOutputParameterValue<int?>();
                        return giftID;
                    }
                }
            );
            return giftID;
        }
        #endregion
    }
}
