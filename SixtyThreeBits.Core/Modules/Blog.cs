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
    public class BlogDataAccess : DataAccessBase
    {
        #region Constructors
        public BlogDataAccess(ConnectionFactory ConnectionFactory) : base(ConnectionFactory)
        {
        }
        #endregion

        #region Methods
        public async Task<BlogPost> GetSingleBlogByID(int? BlogPostID)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSingleBlogByID)}({nameof(BlogPostID)} = {BlogPostID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.BlogPostGetSingleByID(BlogPostID);
                    return Result?.DeserializeJsonTo<BlogPost>();
                }
            });
        }

        public async Task<List<DBCoreDataContext.BlogPostListResultItem>> ListBlog()
        {
            return await TryToReturn($"{nameof(ListBlog)}()", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.BlogPostList().OrderByDescending(Item => Item.BlogPostDate).ToListAsync();
                }
            });
        }

        public async Task<bool> IsBlogSlugUniq(string BlogPostSlug, int? BlogPostID = null)
        {
            return await TryToReturnAsyncTask($"{nameof(IsBlogSlugUniq)}({nameof(BlogPostSlug)} = {BlogPostSlug}, {nameof(BlogPostID)} = {BlogPostID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.BlogPostIsSlugUniq(BlogPostSlug, BlogPostID);
                }
            });
        }

        public async Task<int?> BlogIUD(Enums.DatabaseActions DatabaseAction, int? BlogPostID = null, string BlogPostSlug = null, string BlogPostTitle = null, string BlogPostShortText = null, string BlogPostText = null, string BlogPostAuthorName = null, string BlogPostImageFilename = null, DateTime? BlogPostDate = null, bool? BlogPostIsPublished = null)
        {
            return await TryToReturnAsyncTask($"{nameof(BlogIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(BlogPostID)} = {BlogPostID}, {nameof(BlogPostSlug)} = {BlogPostSlug}, {nameof(BlogPostTitle)} = {BlogPostTitle}, {nameof(BlogPostShortText)} = {BlogPostShortText}, {nameof(BlogPostText)} = {BlogPostText}, {nameof(BlogPostAuthorName)} = {BlogPostAuthorName}, {nameof(BlogPostImageFilename)} = {BlogPostImageFilename}, {nameof(BlogPostDate)} = {BlogPostDate}, {nameof(BlogPostIsPublished)} = {BlogPostIsPublished})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    BlogPostID = await db.BlogIUD(DatabaseAction, BlogPostID, BlogPostSlug, BlogPostTitle, BlogPostShortText, BlogPostText, BlogPostAuthorName, BlogPostImageFilename, BlogPostDate, BlogPostIsPublished);
                    return BlogPostID;
                }
            });
        }
        #endregion
    }

    public class BlogPost
    {
        #region Properties
        public int? BlogPostID { get; set; }
        public string BlogPostSlug { get; set; }
        public string BlogPostTitle { get; set; }
        public string BlogPostShortText { get; set; }
        public string BlogPostText { get; set; }
        public string BlogPostAuthorName { get; set; }
        public string BlogPostImageFilename { get; set; }
        public DateTime? BlogPostDate { get; set; }
        public bool BlogPostIsPublished { get; set; }
        #endregion
    }
}
