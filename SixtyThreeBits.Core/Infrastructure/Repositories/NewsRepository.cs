using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.Infrastructure.Base;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Database.Core;
using SixtyThreeBits.Core.Infrastructure.DTO;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class NewsRepository : RepositoryBase
    {
        #region Constructors
        public NewsRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DBQueriesDataContext.NewsListEntity, NewsDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods
        public async Task<NewsDTO> NewsGetSingleByID(int? newsID)
        {
            return await TryToReturnAsyncTask($"{nameof(NewsGetSingleByID)}({nameof(newsID)} = {newsID})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    var Result = await db.NewsGetSingleByID(newsID);
                    return Result?.DeserializeJsonTo<NewsDTO>();
                }
            });
        }

        public async Task<bool> NewsIsSlugUniq(string newsSlug, int? newsID = null)
        {
            return await TryToReturnAsyncTask($"{nameof(NewsIsSlugUniq)}({nameof(newsSlug)} = {newsSlug}, {nameof(newsID)} = {newsID})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return await db.NewsIsSlugUniq(newsSlug, newsID);
                }
            });
        }

        public async Task<int?> NewsIUD(Enums.DatabaseActions databaseAction, int? newsID = null, string newsSlug = null, string newsTitle = null, string newsTitleEng = null, string newsText = null, string newsTextEng = null, string newsShortDescription = null, string newsShortDescriptionEng = null, string newsImageFilename = null, DateTime? newsDatePublished = null, bool? newsIsPublished = null)
        {
            return await TryToReturnAsyncTask($"{nameof(NewsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(newsID)} = {newsID}, {nameof(newsSlug)} = {newsSlug}, {nameof(newsTitle)} = {newsTitle}, {nameof(newsTitleEng)} = {newsTitleEng}, {nameof(newsText)} = {newsText}, {nameof(newsTextEng)} = {newsTextEng}, {nameof(newsShortDescription)} = {newsShortDescription}, {nameof(newsShortDescriptionEng)} = {newsShortDescriptionEng}, {nameof(newsImageFilename)} = {newsImageFilename}, {nameof(newsDatePublished)} = {newsDatePublished})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    newsID = await db.NewsIUD(databaseAction, newsID, newsSlug, newsTitle, newsTitleEng, newsText, newsTextEng, newsShortDescription, newsShortDescriptionEng, newsImageFilename, newsDatePublished, newsIsPublished);
                    return newsID;
                }
            });
        }

        public async Task<List<NewsDTO>> NewsList()
        {
            return await TryToReturnAsyncTask($"{nameof(NewsList)}()", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return (await db.NewsList().OrderByDescending(item => item.NewsDateCreated).ToListAsync())?.Select(item => _mapper.Map<NewsDTO>(item)).ToList();
                }
            });
        }
        #endregion
    }        
}
