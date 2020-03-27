using Microsoft.EntityFrameworkCore;
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
        #region Methods
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
                            System.IO.Directory.Delete(Folder);
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
                    return (await db.PagesGetSingleByID(PageID, PageIsPublished)).DeserializeTo<Page>();
                }
            });
        }

        public async Task<Page> GetSinglePageBySlug(string PageSlug, bool? IsPublished = null)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSinglePageBySlug)}({nameof(PageSlug)} = {PageSlug}, {nameof(IsPublished)} = {IsPublished})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return (await db.PagesGetSingleBySlug(PageSlug, IsPublished)).DeserializeTo<Page>();
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

        public async Task<int?> PagesIUD(Enums.DatabaseActions DatabaseAction, int? PageID = null, int? PageParentID = null, string PageSlug = null, string PageTitle = null, string PageTitleEng = null, string PageTitleRus = null, string PageText = null, string PageTextEng = null, string PageTextRus = null, string PageData = null, string PageDataEng = null, string PageDataRus = null, string PageShortDescription = null, string PageShortDescriptionEng = null, string PageShortDescriptionRus = null, string PageImageFilename = null, int? PageCode = null, bool? PageIsPublished = null, int? PageSortIndex = null, bool? PageIsMenuItem = null)
        {
            return await TryToReturnAsyncTask($"{nameof(PagesIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(PageID)} = {PageID}, {nameof(PageParentID)} = {PageParentID}, {nameof(PageSlug)} = {PageSlug}, {nameof(PageTitle)} = {PageTitle}, {nameof(PageTitleEng)} = {PageTitleEng}, {nameof(PageTitleRus)} = {PageTitleRus}, {nameof(PageText)} = {PageText}, {nameof(PageTextEng)} = {PageTextEng}, {nameof(PageTextRus)} = {PageTextRus}, {nameof(PageData)} = {PageData}, {nameof(PageDataEng)} = {PageDataEng}, {nameof(PageDataRus)} = {PageDataRus}, {nameof(PageShortDescription)} = {PageShortDescription}, {nameof(PageShortDescriptionEng)} = {PageShortDescriptionEng}, {nameof(PageShortDescriptionRus)} = {PageShortDescriptionRus}, {nameof(PageImageFilename)} = {PageImageFilename}, {nameof(PageCode)} = {PageCode}, {nameof(PageIsPublished)} = {PageIsPublished}, {nameof(PageSortIndex)} = {PageSortIndex}, {nameof(PageIsMenuItem)} = {PageIsMenuItem})", async () =>
            {
                using(var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    if(DatabaseAction == Enums.DatabaseActions.CREATE)
                    {
                        var DBItem = new Pages();
                        DBItem.PageParentID = PageParentID;
                        DBItem.PageSlug = PageSlug;
                        DBItem.PageTitle = PageTitle;
                        DBItem.PageTitleEng = PageTitleEng;
                        DBItem.PageTitleRus = PageTitleRus;
                        DBItem.PageText = PageText;
                        DBItem.PageTextEng = PageTextEng;
                        DBItem.PageTextRus = PageTextRus;
                        DBItem.PageData = PageData;
                        DBItem.PageDataEng = PageDataEng;
                        DBItem.PageDataRus = PageDataRus;
                        DBItem.PageShortDescription = PageShortDescription;
                        DBItem.PageShortDescriptionEng = PageShortDescriptionEng;
                        DBItem.PageShortDescriptionRus = PageShortDescriptionRus;
                        DBItem.PageImageFilename = PageImageFilename;
                        DBItem.PageCode = PageCode;
                        DBItem.PageIsPublished = PageIsPublished == true;
                        DBItem.PageIsMenuItem = PageIsMenuItem == true;

                        await db.Pages.AddAsync(DBItem);
                        await db.SaveChangesAsync();

                        PageID = DBItem.PageID;

                        System.IO.Directory.CreateDirectory($"{AppSettings.UploadFolderPhysicalPath}{Page.FolderName}\\{PageID}");
                    }
                    else if(DatabaseAction == Enums.DatabaseActions.UPDATE)
                    {
                        var DBItem = await db.Pages.FirstOrDefaultAsync(Item => Item.PageID == PageID);
                        if (DBItem != null)
                        {
                            DBItem.PageParentID = PageParentID == Constants.NullValueFor.Int ? null : PageParentID ?? DBItem.PageParentID;
                            DBItem.PageTitle = PageTitle ?? DBItem.PageTitle;
                            DBItem.PageTitleEng = PageTitleEng == Constants.NullValueFor.String ? null : PageTitleEng ?? DBItem.PageTitleEng;
                            DBItem.PageTitleRus = PageTitleRus == Constants.NullValueFor.String ? null : PageTitleRus ?? DBItem.PageTitleRus; 
                            DBItem.PageText = PageText == Constants.NullValueFor.String ? null : PageText ?? DBItem.PageText;
                            DBItem.PageTextEng = PageTextEng == Constants.NullValueFor.String ? null : PageTextEng ?? DBItem.PageTextEng;
                            DBItem.PageTextRus = PageTextRus == Constants.NullValueFor.String ? null : PageTextRus ?? DBItem.PageTextRus;
                            DBItem.PageData = PageData == Constants.NullValueFor.String ? null : PageData ?? DBItem.PageData;
                            DBItem.PageDataEng = PageDataEng == Constants.NullValueFor.String ? null : PageDataEng ?? DBItem.PageDataEng;
                            DBItem.PageDataRus = PageDataRus == Constants.NullValueFor.String ? null : PageDataRus ?? DBItem.PageDataRus;
                            DBItem.PageShortDescription = PageShortDescription == Constants.NullValueFor.String ? null : PageShortDescription ?? DBItem.PageShortDescription;
                            DBItem.PageShortDescriptionEng = PageShortDescriptionEng == Constants.NullValueFor.String ? null : PageShortDescriptionEng ?? DBItem.PageShortDescriptionEng;
                            DBItem.PageShortDescriptionRus = PageShortDescriptionRus == Constants.NullValueFor.String ? null : PageShortDescriptionRus ?? DBItem.PageShortDescriptionRus;
                            DBItem.PageImageFilename = PageImageFilename == Constants.NullValueFor.String ? null : PageImageFilename ?? DBItem.PageImageFilename;
                            DBItem.PageCode = PageCode == Constants.NullValueFor.Int ? null : PageCode ?? DBItem.PageCode;
                            DBItem.PageIsPublished = PageIsPublished ?? DBItem.PageIsPublished;
                            DBItem.PageIsMenuItem = PageIsMenuItem ?? DBItem.PageIsMenuItem;

                            db.Pages.Update(DBItem);
                            await db.SaveChangesAsync();
                        }
                    }

                    return PageID;
                }
            });
        }

        public async Task<List<Pages>> ListPages()
        {
            return await TryToReturnAsyncTask($"{nameof(ListPages)}()", async () =>
            {
                using(var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.Pages.OrderBy(Item => Item.PageSortIndex).ToListAsync();
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
        public string GetFolderPhysicalPath(int? PageID)
        {
            return $"{AppSettings.UploadFolderPhysicalPath}{FolderName}\\{PageID}\\";
        }
        #endregion
    }
}
