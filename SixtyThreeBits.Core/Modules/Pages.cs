using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class PagesDataAccess : DataAccessBase
    {
        #region Properties
        AppSettingsCollection AppSettings;
        public const string FolderName = "pages";
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
                        var Folder = GetPageFolderPhysicalPath(Item.PageID);
                        if (System.IO.Directory.Exists(Folder))
                        {
                            System.IO.Directory.Delete(Folder, true);
                        }
                    }

                    await db.PagesDeleteRecursive(PageID);
                }
            });
        }

        public string GetPageFolderPhysicalPath(int? PageID)
        {
            return $"{AppSettings.UploadFolderPhysicalPath}{FolderName}\\{PageID}\\";
        }

        public string GetPageFolderHttpPath(int? PageID)
        {
            return $"{AppSettings.UploadFolderVirtualPath}{FolderName}/{PageID}/";
        }

        public string GetPageFileHttpPath(int? PageID, string Filename)
        {
            if (string.IsNullOrWhiteSpace(Filename))
            {
                return null;
            }
            else
            {
                return $"{GetPageFolderHttpPath(PageID)}{Filename}";
            }
        }

        public async Task<Page> GetSinglePageByID(int? PageID, bool? PageIsPublished = null)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSinglePageByID)}({nameof(PageID)} = {PageID}, {nameof(PageIsPublished)} = {PageIsPublished})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.PagesGetSingleByID(PageID, PageIsPublished);
                    return Result?.FromJsonTo<Page>();
                }
            });
        }

        public async Task<Page> GetSinglePageBySlugHierarchy(string PageSlug, bool? IsPublished = null)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSinglePageBySlugHierarchy)}({nameof(PageSlug)} = {PageSlug}, {nameof(IsPublished)} = {IsPublished})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.PagesGetSingleBySlugHierarchy(PageSlug, IsPublished);
                    return Result?.FromJsonTo<Page>();
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

        public async Task<int?> PagesIUD(Enums.DatabaseActions DatabaseAction, int? PageID = null, int? PageParentID = null, string PageSlug = null, string PageTitle = null, string PageTitleEng = null, string PageTitleRus = null, string PageText = null, string PageTextEng = null, string PageTextRus = null, string PageTextHeaderHtml = null, string PageTextHeaderHtmlEng = null, string PageTextHeaderHtmlRus = null, string PageTextFooterHtml = null, string PageTextFooterHtmlEng = null, string PageTextFooterHtmlRus = null, string PageData = null, string PageDataEng = null, string PageDataRus = null, string PageShortDescription = null, string PageShortDescriptionEng = null, string PageShortDescriptionRus = null, string PageImageFilename = null, int? PageCode = null, bool? PageIsPublished = null, int? PageSortIndex = null, bool? PageIsMenuItem = null, bool? PageIsFooterItem = null, bool? PageIsExternalUrl = null, string PageExternalUrl = null)
        {
            return await TryToReturnAsyncTask($"{nameof(PagesIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(PageID)} = {PageID}, {nameof(PageParentID)} = {PageParentID}, {nameof(PageSlug)} = {PageSlug}, {nameof(PageTitle)} = {PageTitle}, {nameof(PageTitleEng)} = {PageTitleEng}, {nameof(PageTitleRus)} = {PageTitleRus}, {nameof(PageText)} = {PageText}, {nameof(PageTextEng)} = {PageTextEng}, {nameof(PageTextRus)} = {PageTextRus}, {nameof(PageTextHeaderHtml)} = {PageTextHeaderHtml}, {nameof(PageTextHeaderHtmlEng)} = {PageTextHeaderHtmlEng}, {nameof(PageTextHeaderHtmlRus)} = {PageTextHeaderHtmlRus}, {nameof(PageTextFooterHtml)} = {PageTextFooterHtml}, {nameof(PageTextFooterHtmlEng)} = {PageTextFooterHtmlEng}, {nameof(PageTextFooterHtmlRus)} = {PageTextFooterHtmlRus}, {nameof(PageData)} = {PageData}, {nameof(PageDataEng)} = {PageDataEng}, {nameof(PageDataRus)} = {PageDataRus}, {nameof(PageShortDescription)} = {PageShortDescription}, {nameof(PageShortDescriptionEng)} = {PageShortDescriptionEng}, {nameof(PageShortDescriptionRus)} = {PageShortDescriptionRus}, {nameof(PageImageFilename)} = {PageImageFilename}, {nameof(PageCode)} = {PageCode}, {nameof(PageIsPublished)} = {PageIsPublished}, {nameof(PageSortIndex)} = {PageSortIndex}, {nameof(PageIsMenuItem)} = {PageIsMenuItem}, {nameof(PageIsFooterItem)} = {PageIsFooterItem}, {nameof(PageIsExternalUrl)} = {PageIsExternalUrl}, {nameof(PageExternalUrl)} = {PageExternalUrl})", async () =>
            {
                if (DatabaseAction == Enums.DatabaseActions.DELETE)
                {
                    var Folder = GetPageFolderPhysicalPath(PageID);
                    if (System.IO.Directory.Exists(Folder))
                    {
                        System.IO.Directory.Delete(Folder, true);
                    }
                }
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {

                    PageID = await db.PagesIUD(DatabaseAction, PageID, PageParentID, PageSlug, PageTitle, PageTitleEng, PageTitleRus, PageText, PageTextEng, PageTextRus, PageTextHeaderHtml, PageTextHeaderHtmlEng, PageTextHeaderHtmlRus, PageTextFooterHtml, PageTextFooterHtmlEng, PageTextFooterHtmlRus, PageData, PageDataEng, PageDataRus, PageShortDescription, PageShortDescriptionEng, PageShortDescriptionRus, PageImageFilename, PageCode, PageIsPublished, PageSortIndex, PageIsMenuItem, PageIsFooterItem, PageIsExternalUrl, PageExternalUrl);


                    if (DatabaseAction == Enums.DatabaseActions.CREATE)
                    {
                        var Folder = GetPageFolderPhysicalPath(PageID);
                        System.IO.Directory.CreateDirectory(Folder);
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
        public int? PageID { get; set; }
        public int? ParentID { get; set; }
        public string PageSlug { get; set; }
        public string PageSlugHierarchy { get; set; }
        public string PageTitle { get; set; }
        public string PageTitleEng { get; set; }
        public string PageTitleRus { get; set; }
        public string PageText { get; set; }
        public string PageTextEng { get; set; }
        public string PageTextRus { get; set; }
        public string PageTextHeaderHtml { get; set; }
        public string PageTextHeaderHtmlEng { get; set; }
        public string PageTextHeaderHtmlRus { get; set; }
        public string PageTextFooterHtml { get; set; }
        public string PageTextFooterHtmlEng { get; set; }
        public string PageTextFooterHtmlRus { get; set; }
        public string PageData { get; set; }
        public string PageDataEng { get; set; }
        public string PageDataRus { get; set; }
        public string PageShortDescription { get; set; }
        public string PageShortDescriptionEng { get; set; }
        public string PageShortDescriptionRus { get; set; }
        public string PageImageFilename { get; set; }        
        public int? PageCode { get; set; }
        public bool PageIsPublished { get; set; }
        public int? PageSortIndex { get; set; }
        public bool PageIsMenuItem { get; set; }
        public bool PageIsFooterItem { get; set; }
        public bool PageIsExternalUrl { get; set; }
        public string PageExternalUrl { get; set; }
        #endregion        
    }
}
