using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class NewsRepository : RepositoryBase
    {
        #region Constructors
        public NewsRepository(DbContextFactory connectionFactory) : base(connectionFactory)
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
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

        public async Task<int?> NewsIUD(Enums.DatabaseActions databaseAction, int? newsID = null, string newsSlug = null, string newsTitle = null, string newsTitleEng = null, string newsText = null, string newsTextEng = null, string newsShortDescription = null, string newsShortDescriptionEng = null, string newsImageFilename = null, DateTime? newsDatePublished = null, bool? newsIsPublished = null)
        {
            newsID = await TryToReturnAsyncTask(
                logString: $"{nameof(NewsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(newsID)} = {newsID}, {nameof(newsSlug)} = {newsSlug}, {nameof(newsTitle)} = {newsTitle}, {nameof(newsTitleEng)} = {newsTitleEng}, {nameof(newsText)} = {newsText}, {nameof(newsTextEng)} = {newsTextEng}, {nameof(newsShortDescription)} = {newsShortDescription}, {nameof(newsShortDescriptionEng)} = {newsShortDescriptionEng}, {nameof(newsImageFilename)} = {newsImageFilename}, {nameof(newsDatePublished)} = {newsDatePublished})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(NewsIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                                newsID.ToSqlOutputParameter(nameof(newsID),SqlDbType.Int),
                                newsSlug.ToSqlParameter(nameof(newsSlug),SqlDbType.NVarChar),
                                newsTitle.ToSqlParameter(nameof(newsTitle),SqlDbType.NVarChar),
                                newsTitleEng.ToSqlParameter(nameof(newsTitleEng),SqlDbType.NVarChar),
                                newsText.ToSqlParameter(nameof(newsText),SqlDbType.NVarChar),
                                newsTextEng.ToSqlParameter(nameof(newsTextEng),SqlDbType.NVarChar),
                                newsShortDescription.ToSqlParameter(nameof(newsShortDescription),SqlDbType.NVarChar),
                                newsShortDescriptionEng.ToSqlParameter(nameof(newsShortDescriptionEng),SqlDbType.NVarChar),
                                newsImageFilename.ToSqlParameter(nameof(newsImageFilename),SqlDbType.NVarChar),
                                newsDatePublished.ToSqlParameter(nameof(newsDatePublished),SqlDbType.DateTime),
                                newsIsPublished.ToSqlParameter(nameof(newsIsPublished),SqlDbType.Bit)
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

        public async Task<List<NewsDTO>> NewsList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(NewsList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(NewsList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<NewsDTO>();
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
