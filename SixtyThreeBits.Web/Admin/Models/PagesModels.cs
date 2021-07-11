using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Services;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Reusables.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class PagesTreeModel : WebProjectModelBase
    {        
        #region Methods
        public async Task<PageViewModel> GetPageViewModel()
        {
            var AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Pages.Update);
            var AllowAddNew = User.HasPermission(ControllerActionRouteNames.Admin.Pages.AddNew);
            var AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Pages.Delete);

            var ViewModel = new PageViewModel();

            ViewModel.Pages = (await DataAccessFactory.Pages.ListPages())?.Select(Item => new TreeNodeItem
            {
                NodeID = Item.PageID.ToString(),
                ParentID = Item.PageParentID.HasValue ? Item.PageParentID.ToString() : null,
                NavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.Properties, new { Item.PageID }),
                Caption = Item.PageTitle,
                IsToggler1Checked = Item.PageIsPublished,
                IsToggler2Checked = Item.PageIsMenuItem,
                IsToggler3Checked = Item.PageIsFooterItem,
                ShowAddNewButton = AllowAddNew,
                ShowDeleteButton = AllowDelete,
                ShowToggler1 = AllowUpdate,
                ShowToggler2 = AllowUpdate,
                ShowToggler3 = AllowUpdate
            }).ToList();
            
            ViewModel.ShowAddNewButton = AllowAddNew;
            ViewModel.UrlCreateNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.AddNew);
            ViewModel.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Update);
            ViewModel.UrlSyncParentsAndSortIndexes = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.SyncParentsAndSortIndexes);
            ViewModel.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Delete);
            if (ViewModel.HasPages)
            {
                ViewModel.Pages.ToRecursive(IDPropertyName: nameof(TreeNodeItem.NodeID), nameof(TreeNodeItem.ParentID), nameof(TreeNodeItem.Children));
            }

                return ViewModel;
        }

        public async Task<AjaxResponse> CreatePage(int? PageParentID, string PageTitle)
        {
            var AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Pages.Update);
            var AllowAddNew = User.HasPermission(ControllerActionRouteNames.Admin.Pages.AddNew);
            var AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Pages.Delete);

            
            TreeNodeItem Node = null;


            var PageID = await DataAccessFactory.Pages.PagesIUD(
                DatabaseAction: Enums.DatabaseActions.CREATE,
                PageParentID: PageParentID,
                PageSlug: System.Guid.NewGuid().ToString(),
                PageTitle: PageTitle,
                PageIsMenuItem: false,
                PageIsPublished: false,
                PageIsFooterItem: false
            );

            if (PageID > 0)
            {
                var DBItem = await DataAccessFactory.Pages.GetSinglePageByID(PageID);
                Node = new TreeNodeItem();
                if(DBItem!=null)
                {
                    Node.NodeID = PageID.ToString();
                    Node.ParentID = PageParentID.HasValue ? PageParentID.ToString() : null;
                    Node.Caption = DBItem.PageTitle;
                    Node.ShowToggler1 = AllowUpdate;
                    Node.ShowToggler2 = AllowUpdate;
                    Node.ShowToggler3 = AllowUpdate;
                    Node.NavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.Properties, new { PageID = PageID });
                    Node.ShowAddNewButton = AllowAddNew;
                    Node.ShowDeleteButton = AllowDelete;                                       
                };
            }

            var AR = new AjaxResponse();
            
            if (Node != null)
            {
                AR.IsSuccess = true;
                AR.Data = Node;
            }

            return AR;
        }

        public async Task<AjaxResponse> DeleteRecursive(int? PageID)
        {
            await DataAccessFactory.Pages.DeleteRecursive(PageID);
            
            return new AjaxResponse
            {
                IsSuccess = !DataAccessFactory.Pages.IsError
            };
        }

        public async Task<AjaxResponse> UpdatePage(int? PageID, string PageTitle = null, bool? PageIsPublished = null, bool? PageIsMenuItem = null, bool? PageIsFooterItem = null)
        {
            var AR = new AjaxResponse();
            await DataAccessFactory.Pages.PagesIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                PageID: PageID,
                PageTitle: PageTitle,
                PageIsPublished: PageIsPublished,
                PageIsMenuItem: PageIsMenuItem,
                PageIsFooterItem: PageIsFooterItem
            );
            AR.IsSuccess = !DataAccessFactory.Pages.IsError;

            return AR;
        }

        public async Task<AjaxResponse> SyncParentsAndSortIndexes(SyncSortIndexesModel SubmitModel)
        {
            await DataAccessFactory.Pages.PagesSyncParentsAndSortIndexes(SubmitModel.SortIndexes);

            return new AjaxResponse
            {
                IsSuccess = !DataAccessFactory.Pages.IsError
            };
        }
        #endregion

        public class PageViewModel
        {
            #region Properties
            public bool HasPages => Pages != null && Pages.Count > 0;
            public List<TreeNodeItem> Pages { get; set; }
            public bool ShowAddNewButton { get; set; }

            #region Urls
            public string UrlCreateNew { get; set; }
            public string UrlUpdate { get; set; }
            public string UrlSyncParentsAndSortIndexes { get; set; }
            public string UrlDelete { get; set; }
            #endregion

            #region Texts
            public readonly string TextConfirmDeleteRecord = Resources.TextConfirmDelete;
            public readonly string TextConfirmDeleteRecursive = Resources.TextConfirmDeleteRecursive;
            public readonly string ValidationRequired = Resources.ValidationRequired;
            #endregion
            #endregion
        }
    }

    public class PageModelBase : WebProjectModelBase
    {
        #region Properties
        public Page DBItemPage { get; set; } 
        #endregion
    }

    public class PageModel : PageModelBase
    {
        #region Methods
        public PagePropertiesViewModel GetPagePropertiesViewModel(PagePropertiesViewModel ViewModel)
        {
            if (ViewModel == null)
            {
                ViewModel = new PagePropertiesViewModel();
                ViewModel.PageIsPublished = DBItemPage.PageIsPublished;
                ViewModel.PageIsMenuItem = DBItemPage.PageIsMenuItem;
                ViewModel.PageIsFooterItem = DBItemPage.PageIsFooterItem;
                ViewModel.PageIsExternalUrl = DBItemPage.PageIsExternalUrl;
                ViewModel.PageExternalUrl = DBItemPage.PageExternalUrl;
                ViewModel.PageSlug = DBItemPage.PageSlug;
                ViewModel.PageTitle = DBItemPage.PageTitle;
                ViewModel.PageTitleEng = DBItemPage.PageTitleEng;
                ViewModel.PageTitleRus = DBItemPage.PageTitleRus;
                ViewModel.PageShortDescription = DBItemPage.PageShortDescription;
                ViewModel.PageShortDescriptionEng = DBItemPage.PageShortDescriptionEng;
                ViewModel.PageShortDescriptionRus = DBItemPage.PageShortDescriptionRus;
            }

            ViewModel.PageImageFilename = DBItemPage.PageImageFilename;
            ViewModel.PageImageHttpPath = DataAccessFactory.Pages.GetPageFileHttpPath(DBItemPage.PageID, DBItemPage.PageImageFilename);

            ViewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.DeleteImage, values: new { PageID = DBItemPage.PageID });

            return ViewModel;
        }

        public void ValidatePagePropertiesViewModel(PagePropertiesViewModel ViewModel)
        {
            ViewModel.Errors = new List<SimpleKeyValue<string, string>>
            {
                Validation.ValidateRequired(ErrorKey: Validation.GetJQueryNameSelectorFor(nameof(ViewModel.PageTitle)), ValueToValidate:ViewModel.PageTitle),
                Validation.ValidateRequired(ErrorKey: Validation.GetJQueryNameSelectorFor(nameof(ViewModel.PageSlug)), ValueToValidate:ViewModel.PageSlug)
            };
            ViewModel.Errors.RemoveAll(Item => Item == null);
        }

        public async Task<bool> SavePageProperties(PagePropertiesViewModel ViewModel)
        {
            var HasPageImage = ViewModel.PageImageFile?.Length > 0;
            var PageImageFilename = HasPageImage ? GetFilenameFromUploadedFile(ViewModel.PageImageFile) : null;
            if (HasPageImage)
            {
                Utilities.DeleteUploadedFile(DBItemPage.PageImageFilename, DataAccessFactory.Pages.GetPageFolderPhysicalPath(DBItemPage.PageID));
            }

            await DataAccessFactory.Pages.PagesIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                PageID: DBItemPage.PageID,
                PageSlug: ViewModel.PageSlug,
                PageTitle: ViewModel.PageTitle,
                PageTitleEng: ViewModel.PageTitleEng,
                PageTitleRus: ViewModel.PageTitleRus,
                PageShortDescription: ViewModel.PageShortDescription,
                PageShortDescriptionEng: ViewModel.PageShortDescriptionEng,
                PageShortDescriptionRus: ViewModel.PageShortDescriptionRus,
                PageImageFilename: PageImageFilename,
                PageIsPublished: ViewModel.PageIsPublished,
                PageIsMenuItem: ViewModel.PageIsMenuItem,
                PageIsFooterItem: ViewModel.PageIsFooterItem,
                PageIsExternalUrl: ViewModel.PageIsExternalUrl,
                PageExternalUrl: ViewModel.PageExternalUrl
            );

            if (!DataAccessFactory.Pages.IsError)
            {
                ViewModel.IsSaved = true;
                if (HasPageImage)
                {
                    await SaveUploadedFile(PostedFile: ViewModel.PageImageFile, Filename: PageImageFilename, FolderPhysicalPath: DataAccessFactory.Pages.GetPageFolderPhysicalPath(DBItemPage.PageID));
                }
            }

            return ViewModel.IsSaved;
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            Utilities.DeleteUploadedFile(DBItemPage.PageImageFilename, DataAccessFactory.Pages.GetPageFolderPhysicalPath(DBItemPage.PageID));

            var AR = new AjaxResponse();
            await DataAccessFactory.Pages.PagesIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                PageID: DBItemPage.PageID,
                PageImageFilename: Constants.NullValueFor.String
            );

            AR.IsSuccess = !DataAccessFactory.Pages.IsError;

            return AR;
        }

        public PageBuilderViewModel GetPageBuilderViewModel(int? PageID, string Language)
        {
            if (string.IsNullOrWhiteSpace(Language))
            {
                Language = Enums.Languages.GEORGIAN;
            }

            var ViewModel = new PageBuilderViewModel();
            ViewModel.PageTitle = Utilities.GetValuesByLanguage(Language, DBItemPage.PageTitle, DBItemPage.PageTitleEng, DBItemPage.PageTitleRus);
            ViewModel.PageSlug = DBItemPage.PageSlug;
            ViewModel.PageText = Utilities.GetValuesByLanguage(Language, DBItemPage.PageText, DBItemPage.PageTextEng, DBItemPage.PageTextRus);
            ViewModel.PageData = Utilities.GetValuesByLanguage(Language, DBItemPage.PageData, DBItemPage.PageDataEng, DBItemPage.PageDataRus) ?? "[]";
            ViewModel.IsPublished = DBItemPage.PageIsPublished;
            ViewModel.Language = Language;
            ViewModel.UrlBack = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Index);
            ViewModel.UrlPreview = Url.RouteUrl(ControllerActionRouteNames.Website.Pages.Page, new { PageSlugHierarchy = DBItemPage.PageSlugHierarchy });
            ViewModel.UrlSave = UrlCurrentPage;
            ViewModel.UrlFileManager = GetFileManagerUrl(DataAccessFactory.Pages.GetPageFolderPhysicalPath(DBItemPage.PageID), DataAccessFactory.Pages.GetPageFolderHttpPath(DBItemPage.PageID));
            ViewModel.UrlPdfViewer = Url.RouteUrl(ControllerActionRouteNames.Website.FileViewer.Pdf);

            ViewModel.SelectedLanguage = Utilities.GetValuesByLanguage(Language, Enums.Languages.GEORGIAN, Enums.Languages.ENGLISH, Enums.Languages.RUSSIAN);
            ViewModel.LanguageOptions = new List<SimpleKeyValue<string, string>>
            {
                new SimpleKeyValue<string, string>{ Key = nameof(Enums.Languages.GEORGIAN), Value = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.BuilderLanguage, new { PageID, Language = Enums.Languages.GEORGIAN}), IsSelected = Language == Enums.Languages.GEORGIAN },
                new SimpleKeyValue<string, string>{ Key = nameof(Enums.Languages.ENGLISH), Value = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.BuilderLanguage, new { PageID, Language = Enums.Languages.ENGLISH}), IsSelected = Language == Enums.Languages.ENGLISH},
                new SimpleKeyValue<string, string>{ Key = nameof(Enums.Languages.RUSSIAN), Value = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.BuilderLanguage, new { PageID, Language = Enums.Languages.RUSSIAN}), IsSelected = Language == Enums.Languages.RUSSIAN}
            };

            ViewModel.PluginsClient = new PluginsClient();            
            return ViewModel;
        }

        public List<SimpleKeyValue<string, string>> ValidatePageBuilderSubmitModel(PageBuilderSubmitModel SubmitModel)
        {
            var Errors = new List<SimpleKeyValue<string, string>>
            {
                Validation.ValidateRequired(ErrorKey: Validation.GetJQueryNameSelectorFor(nameof(SubmitModel.PageTitle)), ValueToValidate: SubmitModel.PageTitle)
            };
            

            Errors.RemoveAll(Item => Item == null);

            return Errors;
        }

        public async Task<AjaxResponse> SavePageBuilder(PageBuilderSubmitModel SubmitModel)
        {
            var AR = new AjaxResponse();

            switch (SubmitModel.Language)
            {
                case Enums.Languages.GEORGIAN:
                    {
                        await DataAccessFactory.Pages.PagesIUD(
                            DatabaseAction: Enums.DatabaseActions.UPDATE,
                            PageID: DBItemPage.PageID,
                            PageTitle: SubmitModel.PageTitle ?? Constants.NullValueFor.String,
                            PageText: SubmitModel.PageText ?? Constants.NullValueFor.String,
                            PageTextHeaderHtml: SubmitModel.HeaderSectionHtml ?? Constants.NullValueFor.String,
                            PageTextFooterHtml: SubmitModel.FooterSectionHtml ?? Constants.NullValueFor.String,
                            PageData: SubmitModel.PageData ?? Constants.NullValueFor.String
                        );
                        break;
                    }
                case Enums.Languages.ENGLISH:
                    {
                        await DataAccessFactory.Pages.PagesIUD(
                            DatabaseAction: Enums.DatabaseActions.UPDATE,
                            PageID: DBItemPage.PageID,
                            PageTitleEng: SubmitModel.PageTitle ?? Constants.NullValueFor.String,
                            PageTextEng: SubmitModel.PageText ?? Constants.NullValueFor.String,
                            PageTextHeaderHtmlEng: SubmitModel.HeaderSectionHtml ?? Constants.NullValueFor.String,
                            PageTextFooterHtmlEng: SubmitModel.FooterSectionHtml ?? Constants.NullValueFor.String,
                            PageDataEng: SubmitModel.PageData ?? Constants.NullValueFor.String
                        );
                        break;
                    }
                case Enums.Languages.RUSSIAN:
                    {
                        await DataAccessFactory.Pages.PagesIUD(
                            DatabaseAction: Enums.DatabaseActions.UPDATE,
                            PageID: DBItemPage.PageID,
                            PageTitleRus: SubmitModel.PageTitle ?? Constants.NullValueFor.String,
                            PageTextRus: SubmitModel.PageText ?? Constants.NullValueFor.String,
                            PageTextHeaderHtmlRus: SubmitModel.HeaderSectionHtml ?? Constants.NullValueFor.String,
                            PageTextFooterHtmlRus: SubmitModel.FooterSectionHtml ?? Constants.NullValueFor.String,
                            PageDataRus: SubmitModel.PageData ?? Constants.NullValueFor.String
                        );
                        break;
                    }
            }
            AR.IsSuccess = !DataAccessFactory.Pages.IsError;

            return AR;
        }        
        #endregion

        #region Sub Classes
        public class PagePropertiesViewModel : FormViewModelBase
        {
            
            #region Properties             
            public string PageSlug { get; set; }
            public string PageTitle { get; set; }
            public string PageTitleEng { get; set; }
            public string PageTitleRus { get; set; }
            public string PageShortDescription { get; set; }
            public string PageShortDescriptionEng { get; set; }
            public string PageShortDescriptionRus { get; set; }
            public string PageImageFilename { get; set; }
            public string PageImageHttpPath { get; set; }
            public bool HasPageImage => !string.IsNullOrWhiteSpace(PageImageFilename);

            public bool PageIsPublished { get; set; }
            public bool PageIsMenuItem { get; set; }
            public bool PageIsFooterItem { get; set; }
            public bool PageIsExternalUrl { get; set; }
            public string PageExternalUrl { get; set; }
            public IFormFile PageImageFile { get; set; }            
            public string UrlDeleteImage { get; set; }
            public readonly string TextConfirmDelete = Resources.TextConfirmDelete;
            #endregion
        }

        public class PageBuilderViewModel
        {
            #region Properties
            public PluginsClient PluginsClient { get; set; }
            public string PageTitle { get; set; }
            public string PageSlug { get; set; }
            public string PageText { get; set; }
            public bool IsPublished { get; set; }
            public string Language { get; set; }
            public string PageData { get; set; }
            public string UrlBack { get; set; }
            public string UrlPreview { get; set; }
            public string UrlSave { get; set; }
            public string UrlFileManager { get; set; }
            public string UrlPdfViewer { get; set; }
            public string SelectedLanguage { get; set; }
            public List<SimpleKeyValue<string, string>> LanguageOptions { get; set; }
            public bool HasLanguageOptions => LanguageOptions?.Count > 0;

            public readonly string TextError = Resources.TextError;
            #endregion
        }

        public class PageBuilderSubmitModel
        {
            #region Properties
            public string PageTitle { get; set; }
            public string PageSlug { get; set; }
            public string PageText { get; set; }
            public string Language { get; set; }
            public string PageData { get; set; }
            public string HeaderSectionHtml { get; set; }
            public string FooterSectionHtml { get; set; }
            public bool IsPublished { get; set; }
            #endregion
        }
        #endregion
    }    
}