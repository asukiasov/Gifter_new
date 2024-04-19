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
    public class PagesRepository : RepositoryBase
    {
        #region Contructors
        public PagesRepository(DbContextFactory dbContextFactory) : base(dbContextFactory)
        {
            
        }
        #endregion

        #region Methods
        public async Task PagesDelete(int? pageID)
        {
            await TryExecuteAsyncTask(
                logString:$"{nameof(PagesDelete)}({nameof(pageID)} = {pageID})",
                asyncFuncToTry : async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PagesDelete),
                            sqlParameters:
                            [
                                pageID.ToSqlParameter(nameof(pageID), SqlDbType.Int),
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();                        
                    }
                }
            );
        }

        public async Task<PageDTO> PagesGetSingleByID(int? pageID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(PagesGetSingleByID)}({nameof(pageID)} = {pageID})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PagesGetSingleByID),
                            sqlParameters:
                            [
                                pageID.ToSqlParameter(nameof(pageID), SqlDbType.Int)
                            ]
                        );
                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();                        
                        var result = resultJson?.DeserializeJsonTo<PageDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<PageDTO> PagesGetSingleBySlug(string pageSlug)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(PagesGetSingleByID)}({nameof(pageSlug)} = {pageSlug})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PagesGetSingleBySlug),
                            sqlParameters:
                            [
                                pageSlug.ToSqlParameter(nameof(pageSlug), SqlDbType.NVarChar)
                            ]
                        );
                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();                        
                        var result = resultJson?.DeserializeJsonTo<PageDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<bool> PagesIsSlugUniq(string pageSlug, int? pageID = null)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(PagesIsSlugUniq)}({nameof(pageSlug)} = {pageSlug}, {nameof(pageID)} = {pageID})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PagesIsSlugUniq),
                            sqlParameters:
                            [
                                pageSlug.ToSqlParameter(nameof(pageSlug), SqlDbType.NVarChar),
                                pageID.ToSqlParameter(nameof(pageID), SqlDbType.Int)
                            ]
                        );
                        var result = await sqb.ExecuteScalarValuedFunction<bool>();                        
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> PagesIUD(Enums.DatabaseActions databaseAction, int? pageID, PageIudDTO page)
        {
            var pageJson = page.ToJson();

            pageID = await TryToReturnAsyncTask(
                logString: $"{nameof(PagesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(pageID)} = {pageID}, {nameof(page)} = {pageJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PagesIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                                pageID.ToSqlOutputParameter(nameof(pageID),SqlDbType.Int),
                                pageJson.ToSqlParameter(nameof(pageJson),SqlDbType.NVarChar)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        pageID = sqb.GetNextOutputParameterValue<int?>();
                        return pageID;
                    }
                }
            );
            return pageID;
        }

        public async Task<List<PagesListDTO>> PagesList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(PagesList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PagesList)
                        );
                        var resultQueryable = sqb.ExecuteTableValuedFunction<PagesListDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.PageDateCreated);
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