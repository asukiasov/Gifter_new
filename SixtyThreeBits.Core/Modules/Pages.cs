using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.DB.Tables;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class PagesDataAccess : DataAccessBase
    {
        #region Properties
        AppSettingsCollection AppSettings;
        #endregion

        #region Contructors
        public PagesDataAccess(ConnectionFactory ConnectionFactory, AppSettingsCollection AppSettings) : base(ConnectionFactory)
        {
            this.AppSettings = AppSettings;
        }
        #endregion

        #region Methods
        public async Task DeleteRecursive(int? PageID)
        {
            await TryExecuteAsyncTask($"{nameof(DeleteRecursive)}({nameof(PageID)} = {PageID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var DBItems = db.PagesListForDeleteRecursive(PageID).ToList();
                    foreach (var Item in DBItems)
                    {
                        var Folder = $"{AppSettings.UploadFolderPhysicalPath}{Page.FolderName}\\{Item.PageID}";
                        if (System.IO.Directory.Exists(Folder))
                        {
                            System.IO.Directory.Delete(Folder, true);
                        }
                    }

                    await db.PagesDeleteRecursive(PageID);
                }
            });
        }

        public async Task<Page> GetSinglePageByID(int? PageID, bool? PageIsPublished = null)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSinglePageByID)}({nameof(PageID)} = {PageID}, {nameof(PageIsPublished)} = {PageIsPublished})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.PagesGetSingleByID(PageID, PageIsPublished);
                    return Result?.DeserializeTo<Page>();
                }
            });
        }

        public async Task<Page> GetSinglePageBySlug(string PageSlug, bool? IsPublished = null)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSinglePageBySlug)}({nameof(PageSlug)} = {PageSlug}, {nameof(IsPublished)} = {IsPublished})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.PagesGetSingleBySlug(PageSlug, IsPublished);
                    return Result?.DeserializeTo<Page>();
                }
            });
        }

        public async Task<bool> IsPageSlugUniq(string PageSlug, int? PageID = null)
        {
            return await TryToReturnAsyncTask($"{nameof(IsPageSlugUniq)}({nameof(PageSlug)} = {PageSlug}, {nameof(PageID)} = {PageID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.PagesIsSlugUniq(PageSlug, PageID);
                }
            });
        }

        public async Task PagesSyncParentsAndSortIndexes(List<SyncSortIndexesItem> SortIndexes)
        {
            await TryExecuteAsyncTask($"{nameof(PagesSyncParentsAndSortIndexes)}({nameof(SortIndexes)} = {SortIndexes.ToXml()})", async () =>
            {
                if (SortIndexes?.Count > 0)
                {
                    using (var db = ConnectionFactory.GetDBCoreDataContext())
                    {
                        await db.PagesSyncParentsAndSortIndexes(SortIndexes.ToXml());
                    }
                }
            });
        }

        public async Task<int?> PagesIUD(Enums.DatabaseActions DatabaseAction, int? PageID = null, int? PageParentID = null, string PageSlug = null, string PageTitle = null, string PageTitleEng = null, string PageTitleRus = null, string PageText = null, string PageTextEng = null, string PageTextRus = null, string PageData = null, string PageDataEng = null, string PageDataRus = null, string PageShortDescription = null, string PageShortDescriptionEng = null, string PageShortDescriptionRus = null, string PageImageFilename = null, int? PageCode = null, bool? PageIsPublished = null, int? PageSortIndex = null, bool? PageIsMenuItem = null, bool? PageIsFooterItem = null, bool? PageIsExternalUrl = null, string PageExternalUrl = null)
        {
            return await TryToReturnAsyncTask($"{nameof(PagesIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(PageID)} = {PageID}, {nameof(PageParentID)} = {PageParentID}, {nameof(PageSlug)} = {PageSlug}, {nameof(PageTitle)} = {PageTitle}, {nameof(PageTitleEng)} = {PageTitleEng}, {nameof(PageTitleRus)} = {PageTitleRus}, {nameof(PageText)} = {PageText}, {nameof(PageTextEng)} = {PageTextEng}, {nameof(PageTextRus)} = {PageTextRus}, {nameof(PageData)} = {PageData}, {nameof(PageDataEng)} = {PageDataEng}, {nameof(PageDataRus)} = {PageDataRus}, {nameof(PageShortDescription)} = {PageShortDescription}, {nameof(PageShortDescriptionEng)} = {PageShortDescriptionEng}, {nameof(PageShortDescriptionRus)} = {PageShortDescriptionRus}, {nameof(PageImageFilename)} = {PageImageFilename}, {nameof(PageCode)} = {PageCode}, {nameof(PageIsPublished)} = {PageIsPublished}, {nameof(PageSortIndex)} = {PageSortIndex}, {nameof(PageIsMenuItem)} = {PageIsMenuItem}, {nameof(PageIsFooterItem)} = {PageIsFooterItem}, {nameof(PageIsExternalUrl)} = {PageIsExternalUrl}, {nameof(PageExternalUrl)} = {PageExternalUrl})", async () =>
            {
                if (DatabaseAction == Enums.DatabaseActions.DELETE)
                {
                    var Folder = $"{AppSettings.UploadFolderPhysicalPath}{Page.FolderName}\\{PageID}";
                    if (System.IO.Directory.Exists(Folder))
                    {
                        System.IO.Directory.Delete(Folder, true);
                    }
                }
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {

                    PageID = await db.PagesIUD(DatabaseAction, PageID, PageParentID, PageSlug, PageTitle, PageTitleEng, PageTitleRus, PageText, PageTextEng, PageTextRus, PageData, PageDataEng, PageDataRus, PageShortDescription, PageShortDescriptionEng, PageShortDescriptionRus, PageImageFilename, PageCode, PageIsPublished, PageSortIndex, PageIsMenuItem, PageIsFooterItem, PageIsExternalUrl, PageExternalUrl);


                    if (DatabaseAction == Enums.DatabaseActions.CREATE)
                    {
                        System.IO.Directory.CreateDirectory($"{AppSettings.UploadFolderPhysicalPath}{Page.FolderName}\\{PageID}");
                    }
                    return PageID;
                }
            });
        }

        public async Task<List<DBCoreDataContext.PagesListResultItem>> ListPages(bool? PageIsPublished = null, bool? PageIsMenuItem = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ListPages)}({nameof(PageIsPublished)} = {PageIsPublished}, {nameof(PageIsMenuItem)} = {PageIsMenuItem})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.PagesList(PageIsPublished, PageIsMenuItem).OrderBy(Item => Item.PageSortIndex).ToListAsync();
                }
            });
        }
        #endregion
    }

    public class Page
    {
        #region Properties
        AppSettingsCollection AppSettings;
        public int? PageID { get; set; }
        public int? ParentID { get; set; }
        public string PageSlug { get; set; }
        public string PageTitle { get; set; }
        public string PageTitleEng { get; set; }
        public string PageTitleRus { get; set; }
        public string PageText { get; set; }
        public string PageTextEng { get; set; }
        public string PageTextRus { get; set; }
        public string PageData { get; set; }
        public string PageDataEng { get; set; }
        public string PageDataRus { get; set; }
        public string PageShortDescription { get; set; }
        public string PageShortDescriptionEng { get; set; }
        public string PageShortDescriptionRus { get; set; }
        public string PageImageFilename { get; set; }
        public bool HasPageImage => !string.IsNullOrWhiteSpace(PageImageFilename);
        public string PageImageFilenameHttpPath => HasPageImage ? $"{FolderVirtualPath}{PageImageFilename}" : null;
        public int? PageCode { get; set; }
        public bool PageIsPublished { get; set; }
        public int? PageSortIndex { get; set; }
        public bool PageIsMenuItem { get; set; }
        public bool PageIsFooterItem { get; set; }
        public bool PageIsExternalUrl { get; set; }
        public string PageExternalUrl { get; set; }

        public const string FolderName = "pages";
        public string FolderPhysicalPath => $"{AppSettings.UploadFolderPhysicalPath}{FolderName}\\{PageID}\\";
        public string FolderVirtualPath => $"{AppSettings.UploadFolderVirtualPath}{FolderName}/{PageID}/";
        #endregion

        #region Constructors
        public Page() { }

        public Page(AppSettingsCollection AppSettings)
        {
            this.AppSettings = AppSettings;
        }
        #endregion

        #region Methods
        public void SetAppSettings(AppSettingsCollection AppSettings)
        {
            this.AppSettings = AppSettings;
        }
        public string GetFolderPhysicalPath(int? PageID)
        {
            return $"{AppSettings.UploadFolderPhysicalPath}{FolderName}\\{PageID}\\";
        }
        #endregion
    }
}
