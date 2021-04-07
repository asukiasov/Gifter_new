using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class NewsDataAccess : DataAccessBase
    {        
        #region Constructors
        public NewsDataAccess(ConnectionFactory ConnectionFactory) : base(ConnectionFactory)
        {
            
        }
        #endregion

        #region Methods
        public async Task<News> GetSingleNewsByID(int? NewsID)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSingleNewsByID)}({nameof(NewsID)} = {NewsID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.NewsGetSingleByID(NewsID);
                    return Result?.DeserializeTo<News>();
                }
            });
        }

        public async Task<bool> IsSlugUniq(string NewsSlug, int? NewsID = null)
        {
            return await TryToReturnAsyncTask($"{nameof(IsSlugUniq)}({nameof(NewsSlug)} = {NewsSlug}, {nameof(NewsID)} = {NewsID})", async () =>
            {
                using(var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.NewsIsSlugUniq(NewsSlug, NewsID);
                }
            });
        }

        public async Task<List<DBCoreDataContext.NewsListResultItem>> ListNews()
        {
            return await TryToReturnAsyncTask($"{nameof(ListNews)}()", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.NewsList().OrderByDescending(Item => Item.NewsDatePublished).ToListAsync();
                }
            });
        }

        public async Task<int?> NewsIUD(Enums.DatabaseActions DatabaseAction, int? NewsID = null, string NewsSlug = null, string NewsTitle = null, string NewsTitleEng = null, string NewsTitleRus = null, string NewsText = null, string NewsTextEng = null, string NewsTextRus = null, string NewsShortDescription = null, string NewsShortDescriptionEng = null, string NewsShortDescriptionRus = null, string NewsImageFilename = null, DateTime? NewsDatePublished = null, bool NewsIsPublished = false, DateTime? NewsDateCreated = null)
        {
            return await TryToReturnAsyncTask($"{nameof(NewsIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(NewsID)} = {NewsID}, {nameof(NewsSlug)} = {NewsSlug}, {nameof(NewsTitle)} = {NewsTitle}, {nameof(NewsTitleEng)} = {NewsTitleEng}, {nameof(NewsTitleRus)} = {NewsTitleRus}, {nameof(NewsText)} = {NewsText}, {nameof(NewsTextEng)} = {NewsTextEng}, {nameof(NewsTextRus)} = {NewsTextRus},{nameof(NewsShortDescription)} = {NewsShortDescription},{nameof(NewsShortDescriptionEng)} = {NewsShortDescriptionEng},{nameof(NewsShortDescriptionRus)} = {NewsShortDescriptionRus},{nameof(NewsImageFilename)} = {NewsImageFilename},{nameof(NewsDatePublished)} = {NewsDatePublished},{nameof(NewsIsPublished)} = {NewsIsPublished},{nameof(NewsDateCreated)} = {NewsDateCreated})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    NewsID = await db.NewsIUD(DatabaseAction, NewsID, NewsSlug, NewsTitle, NewsTitleEng, NewsTitleRus, NewsText, NewsTextEng, NewsTextRus, NewsShortDescription, NewsShortDescriptionEng, NewsShortDescriptionRus, NewsImageFilename, NewsDatePublished, NewsIsPublished, NewsDateCreated);
                    return NewsID;
                }
            });
        }
        #endregion

    }
    public class News
    {
        #region Properties
        
        public int? NewsID { get; set; }
        public string NewsSlug { get; set; }
        public string NewsTitle { get; set; }
        public string NewsTitleEng { get; set; }
        public string NewsTitleRus { get; set; }
        public string NewsText { get; set; }
        public string NewsTextEng { get; set; }
        public string NewsTextRus { get; set; }
        public string NewsShortDescription { get; set; }
        public string NewsShortDescriptionEng { get; set; }
        public string NewsShortDescriptionRus { get; set; }
        public string NewsImageFilename { get; set; }
        public DateTime? NewsDatePublished { get; set; }
        public bool NewsIsPublished { get; set; }
        public bool HasNewsImage => !string.IsNullOrWhiteSpace(NewsImageFilename);        
        #endregion        
    }
}
