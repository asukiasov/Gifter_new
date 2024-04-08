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
    public class MenuHeaderRepository : RepositoryBase
    {
        #region Contructors
        public MenuHeaderRepository(DbContextFactory dbContextFactory) : base(dbContextFactory)
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(MenuHeaderGetSingleByID),
                            sqlParameters:
                            [
                                menuHeaderID.ToSqlParameter(nameof(menuHeaderID), SqlDbType.Int)
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(MenuHeaderIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(nameof(databaseAction), SqlDbType.TinyInt),
                                menuHeaderID.ToSqlOutputParameter(nameof(menuHeaderID), SqlDbType.Int),
                                menuHeaderJson.ToSqlParameter(nameof(menuHeaderJson), SqlDbType.NVarChar)
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(MenuHeaderList),
                            sqlParameters:
                            [
                                menuHeaderIsPublished.ToSqlParameter(nameof(menuHeaderIsPublished), SqlDbType.Bit)
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(MenuHeaderSort),
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
