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
    public class MenuHeaderRepository : RepositoryBase
    {
        #region Contructors
        public MenuHeaderRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {            
        }
        #endregion

        #region Methods
        public async Task<MenuHeaderDTO> MenuHeaderGetSingleByID(int? menuHeaderID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(MenuHeaderGetSingleByID)}({nameof(menuHeaderID)} = {menuHeaderID})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(MenuHeaderGetSingleByID),
                            sqlParameters:
                            [
                                menuHeaderID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );
                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<MenuHeaderDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> MenuHeaderIUD(Enums.DatabaseActions databaseAction, int? menuHeaderID, MenuHeaderIudDTO menuHeader)
        {
            var menuHeaderJson = menuHeader.ToJson();

            menuHeaderID = await TryToReturnAsyncTask(
                logString: $"{nameof(MenuHeaderIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(menuHeaderID)} = {menuHeaderID}, {nameof(menuHeader)} = {menuHeaderJson})",
                asyncFuncToTry: async () =>
                {                    
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(MenuHeaderIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(SqlDbType.TinyInt),
                                menuHeaderID.ToSqlParameterOutput(SqlDbType.Int),
                                menuHeaderJson.ToSqlParameter(SqlDbType.NVarChar)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        menuHeaderID = sqb.GetNextOutputParameterValue<int?>();
                        return menuHeaderID;
                    }
                }
            );
            return menuHeaderID;
        }

        public async Task<List<MenuHeaderDTO>> MenuHeaderList(bool? menuHeaderIsPublished = null)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(MenuHeaderList)}({nameof(menuHeaderIsPublished)} = {menuHeaderIsPublished})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(MenuHeaderList),
                            sqlParameters:
                            [
                                menuHeaderIsPublished.ToSqlParameter(SqlDbType.Bit)
                            ]
                        );
                        var resultQueryable = sqb.ExecuteTableValuedFunction<MenuHeaderDTO>();
                        resultQueryable = resultQueryable.OrderBy(Item => Item.MenuHeaderSortIndex);
                        var result = await resultQueryable.ToListAsync();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task MenuHeaderSort(List<SyncSortIndexesDTO> sortIndexes)
        {
            var sortIndexesJson = sortIndexes.ToJson();

            await TryExecuteAsyncTask(
                logString: $"{nameof(MenuHeaderSort)}({nameof(sortIndexes)} = {sortIndexesJson})",
                asyncFuncToTry: async () =>
                {                    
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(MenuHeaderSort),
                            sqlParameters:
                            [                                
                                sortIndexesJson.ToSqlParameter(SqlDbType.NVarChar)
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
