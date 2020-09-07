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
    public class BlogsDataAccess : DataAccessBase
    {
        #region Constructors
        public BlogsDataAccess(ConnectionFactory ConnectionFactory) : base(ConnectionFactory)
        {
        }
        #endregion

        #region Methods
        public async Task<Blog> GetSingleBlogByID(int? BlogID)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSingleBlogByID)}({nameof(BlogID)} = {BlogID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.BlogGetSingleByID(BlogID);
                    return Result?.DeserializeTo<Blog>();
                }
            });
        }

        public async Task<List<DBCoreDataContext.BlogsListResultItem>> ListBlog()
        {
            return await TryToReturnStatic($"{nameof(ListBlog)}()", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.BlogList().OrderByDescending(Item => Item.BlogDate).ToListAsync();
                }
            });
        }

        public async Task<bool> IsBlogSlugUniq(string BlogSlug, int? BlogID = null)
        {
            return await TryToReturnAsyncTask($"{nameof(IsBlogSlugUniq)}({nameof(BlogSlug)} = {BlogSlug}, {nameof(BlogID)} = {BlogID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.BlogIsSlugUniq(BlogSlug, BlogID);
                }
            });
        }

        public async Task<int?> BlogIUD(Enums.DatabaseActions DatabaseAction, int? BlogID = null, string BlogSlug = null, string BlogTitle = null, string BlogText = null, string BlogAuthorName = null, string BlogImageFilename = null, DateTime? BlogDate = null)
        {
            return await TryToReturnStaticAsyncTask($"{nameof(BlogIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(BlogID)} = {BlogID}, {nameof(BlogSlug)} = {BlogSlug}, {nameof(BlogTitle)} = {BlogTitle}, {nameof(BlogText)} = {BlogText}, {nameof(BlogAuthorName)} = {BlogAuthorName}, {nameof(BlogImageFilename)} = {BlogImageFilename}, {nameof(BlogDate)} = {BlogDate})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    BlogID = await db.BlogIUD(DatabaseAction, BlogID, BlogSlug, BlogTitle, BlogText, BlogAuthorName, BlogImageFilename, BlogDate);
                    return BlogID;
                }
            });
        }
        #endregion

    }
    public class Blog
    {
        #region Properties
        AppSettingsCollection AppSettings;
        public int? BlogID { get; set; }
        public string BlogSlug { get; set; }
        public string BlogTitle { get; set; }
        public string BlogText { get; set; }
        public string BlogAuthorName { get; set; }
        public DateTime? BlogDate { get; set; }
        public string BlogImageFilename { get; set; }
        public bool HasBlogImage => !string.IsNullOrWhiteSpace(BlogImageFilename);
        public string BlogImageFilenameHttpPath => HasBlogImage ? $"{FolderVirtualPath}{BlogImageFilename}" : null;
        public string BlogImageHttpPath => HasBlogImage ? $"{BlogImageFilename}" : null;
        public string FolderPhysicalPath => $"{AppSettings.UploadFolderPhysicalPath}\\";
        public string FolderVirtualPath => $"{AppSettings.UploadFolderVirtualPath}/";
        #endregion

        #region Constructors
        public Blog() { }

        public Blog(AppSettingsCollection AppSettings)
        {
            this.AppSettings = AppSettings;
        }
        #endregion

        #region Methods
        public void SetAppSettings(AppSettingsCollection AppSettings)
        {
            this.AppSettings = AppSettings;
        }
        public string GetFolderPhysicalPath()
        {
            return $"{AppSettings.UploadFolderPhysicalPath}\\";
        }
        #endregion
    }
}
