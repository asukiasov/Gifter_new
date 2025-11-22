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
    public class MenuFooterRepository : RepositoryBase
    {
        #region Contructors
        public MenuFooterRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {            
        }
        #endregion

        #region Methods
        public async Task<MenuFooterDTO> MenuFooterGetSingleByID(int? menuFooterID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(MenuFooterGetSingleByID)}({nameof(menuFooterID)} = {menuFooterID})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(MenuFooterGetSingleByID),
                            sqlParameters:
                            [
                                menuFooterID.ToSqlParameter(nameof(menuFooterID), SqlDbType.Int)
                            ]
                        );
                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<MenuFooterDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> MenuFooterIUD(Enums.DatabaseActions databaseAction, int? menuFooterID, MenuFooterIudDTO menuFooter)
        {
            var menuFooterJson = menuFooter.ToJson();

            menuFooterID = await TryToReturnAsyncTask(
                logString: $"{nameof(MenuFooterIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(menuFooterID)} = {menuFooterID}, {nameof(menuFooter)} = {menuFooterJson})",
                asyncFuncToTry: async () =>
                {                    
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(MenuFooterIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(nameof(databaseAction), SqlDbType.TinyInt),
                                menuFooterID.ToSqlParameterOutput(nameof(menuFooterID), SqlDbType.Int),
                                menuFooterJson.ToSqlParameter(nameof(menuFooterJson), SqlDbType.NVarChar)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        menuFooterID = sqb.GetNextOutputParameterValue<int?>();
                        return menuFooterID;
                    }
                }
            );
            return menuFooterID;
        }

        public async Task<List<MenuFooterDTO>> MenuFooterList(bool? menuFooterIsPublished = null)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(MenuFooterList)}({nameof(menuFooterIsPublished)} = {menuFooterIsPublished})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(MenuFooterList),
                            sqlParameters:
                            [
                                menuFooterIsPublished.ToSqlParameter(nameof(menuFooterIsPublished), SqlDbType.Bit)
                            ]
                        );
                        var resultQueryable = sqb.ExecuteTableValuedFunction<MenuFooterDTO>();
                        resultQueryable = resultQueryable.OrderBy(Item => Item.MenuFooterSortIndex);
                        var result = await resultQueryable.ToListAsync();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task MenuFooterSort(List<SyncSortIndexesDTO> sortIndexes)
        {
            var sortIndexesJson = sortIndexes.ToJson();

            await TryExecuteAsyncTask(
                logString: $"{nameof(MenuFooterSort)}({nameof(sortIndexes)} = {sortIndexesJson})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(MenuFooterSort),
                            sqlParameters:
                            [
                                sortIndexesJson.ToSqlParameter(nameof(sortIndexesJson), SqlDbType.NVarChar)
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