using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.Database;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class OrdersRepository : RepositoryBase
    {
        #region Constructors
        public OrdersRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {
        }
        #endregion

        #region Methods
        public async Task<List<OrdersListDTO>> OrdersList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(OrdersList)}()",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(OrdersList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<OrdersListDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.OrderDateCreated);
                        var result = await resultQueryable.ToListAsync();

                        return result;
                    }
                }
            );
            return result;
        }

        public async Task OrdersInsert(int orderUserID, int orderType, int? orderGiftListID = null, int? orderGiftID = null, int? orderTargetUserID = null)
        {
            await TryExecuteAsyncTask(
                logString: $"{nameof(OrdersInsert)}({nameof(orderUserID)} = {orderUserID}, {nameof(orderType)} = {orderType}, {nameof(orderGiftListID)} = {orderGiftListID}, {nameof(orderGiftID)} = {orderGiftID}, {nameof(orderTargetUserID)} = {orderTargetUserID})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(OrdersInsert),
                            sqlParameters:
                            [
                                orderUserID.ToSqlParameter(SqlDbType.Int),
                                orderType.ToSqlParameter(SqlDbType.Int),
                                orderGiftListID.ToSqlParameter(SqlDbType.Int),
                                orderGiftID.ToSqlParameter(SqlDbType.Int),
                                orderTargetUserID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                    }
                }
            );
        }
        #endregion
    }
}
