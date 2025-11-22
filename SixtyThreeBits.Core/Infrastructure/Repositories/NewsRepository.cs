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
    public class NewsRepository : RepositoryBase
    {
        #region Constructors
        public NewsRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {            
        }
        #endregion

        #region Methods
        public async Task<NewsDTO> NewsGetSingleByID(int? newsID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(NewsGetSingleByID)}({nameof(newsID)} = {newsID})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(NewsGetSingleByID),
                            sqlParameters:
                            [
                                newsID.ToSqlParameter(nameof(newsID), SqlDbType.Int)
                            ]
                        );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<NewsDTO>();
                        
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<bool> NewsIsSlugUniq(string newsSlug, int? newsID = null)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(NewsIsSlugUniq)}({nameof(newsSlug)} = {newsSlug}, {nameof(newsID)} = {newsID})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(NewsIsSlugUniq),
                            sqlParameters:
                            [
                                newsSlug.ToSqlParameter(nameof(newsSlug), SqlDbType.NVarChar),
                                newsID.ToSqlParameter(nameof(newsID), SqlDbType.Int)
                            ]
                        );
                        var result = await sqb.ExecuteScalarValuedFunction<bool>();                        
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> NewsIUD(Enums.DatabaseActions databaseAction, int? newsID, NewsIudDTO news)
        {
            var newsJson = news.ToJson();

            newsID = await TryToReturnAsyncTask(
                logString: $"{nameof(NewsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(newsID)} = {newsID}, {nameof(news)} = {newsJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(NewsIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                                newsID.ToSqlParameterOutput(nameof(newsID),SqlDbType.Int),
                                newsJson.ToSqlParameter(nameof(newsJson),SqlDbType.NVarChar)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        newsID = sqb.GetNextOutputParameterValue<int?>();

                        return newsID;                        
                    }
                }
            );
            return newsID;
        }

        public async Task<List<NewsListDTO>> NewsList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(NewsList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(NewsList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<NewsListDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.NewsDateCreated);
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
