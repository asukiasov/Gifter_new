using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.Database;
using System.Collections.Generic;
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
        #endregion
    }
}
