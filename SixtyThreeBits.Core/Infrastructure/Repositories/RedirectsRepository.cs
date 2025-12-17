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
    public class RedirectsRepository : RepositoryBase
    {
        #region Constructors
        public RedirectsRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {            
        }
        #endregion

        #region Methods
        public async Task<int?> RedirectsIUD(Enums.DatabaseActions databaseAction, int? redirectID, RedirectIudDTO redirect)
        {
            var redirectJson = redirect.ToJson();

            redirectID = await TryToReturnAsyncTask(
                logString: $"{nameof(RedirectsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(redirectID)} = {redirectID}, {nameof(redirect)} = {redirectJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(RedirectsIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(SqlDbType.TinyInt),
                                redirectID.ToSqlParameterOutput(SqlDbType.Int),
                                redirectJson.ToSqlParameter(SqlDbType.NVarChar)
                            ]
                         );

                        await sqb.ExecuteStoredProcedure();
                        redirectID = sqb.GetNextOutputParameterValue<int?>();
                        return redirectID;
                    }
                }
            );
            return redirectID;
        }

        public async Task<List<RedirectDTO>> RedirectsList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(RedirectsList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(RedirectsList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<RedirectDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.RedirectDateCreated);
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
