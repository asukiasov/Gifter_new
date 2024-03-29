using AutoMapper;
using Imageflow.Bindings;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static SixtyThreeBits.Core.Infrastructure.Database.DbContextQueries;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class RedirectsRepository : RepositoryBase
    {
        #region Constructors
        public RedirectsRepository(DbContextFactory connectionFactory) : base(connectionFactory)
        {            
        }
        #endregion

        #region Methods
        public async Task<int?> RedirectsIUD(Enums.DatabaseActions databaseAction, int? redirectID = null, string redirectFrom = null, string redirectTo = null)
        {
            redirectID = await TryToReturnAsyncTask(
                logString: $"{nameof(RedirectsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(redirectID)} = {redirectID}, {nameof(redirectFrom)} = {redirectFrom}, {nameof(redirectTo)} = {redirectTo})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(RedirectsIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                                redirectID.ToSqlOutputParameter(nameof(redirectID),SqlDbType.Int),
                                redirectFrom.ToSqlParameter(nameof(redirectFrom),SqlDbType.NVarChar),
                                redirectTo.ToSqlParameter(nameof(redirectTo),SqlDbType.NVarChar),
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
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
