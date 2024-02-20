using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Libraries.FileStorages;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using SixtyThreeBits.Web.Models.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class PagesTreeModel : ModelBase
    {
        #region Methods
        public async Task<PageViewModel> GetPageViewModel()
        {
            var viewModel = new PageViewModel();

            var allowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Pages.Update);
            var allowAddNew = User.HasPermission(ControllerActionRouteNames.Admin.Pages.AddNew);
            var allowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Pages.Delete);

            var repository = RepositoriesFactory.GetPagesRepository();
            viewModel.Pages = (await repository.PagesList())?.Select(item => new TreeNodeItem
            {
                NodeID = item.PageID.ToString(),
                ParentID = item.PageParentID.HasValue ? item.PageParentID.ToString() : null,
                NavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.Properties, new { item.PageID }),
                Caption = item.PageTitle,
                IsToggler1Checked = item.PageIsPublished,
                IsToggler2Checked = item.PageIsMenuItem,
                IsToggler3Checked = item.PageIsFooterItem,
                TextToggler1 = Resources.TextPublished,
                TextToggler2 = Resources.TextMenu,
                TextToggler3 = Resources.TextFooter,
                ShowAddNewButton = allowAddNew,
                ShowDeleteButton = allowDelete,
                ShowToggler1 = allowUpdate,
                ShowToggler2 = allowUpdate,
                ShowToggler3 = allowUpdate
            }).ToList();

            viewModel.ShowAddNewButton = allowAddNew;
            viewModel.UrlCreateNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.AddNew);
            viewModel.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Update);
            viewModel.UrlSyncParentsAndSortIndexes = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.SyncParentsAndSortIndexes);
            viewModel.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Delete);
            if (viewModel.HasPages)
            {
                viewModel.Pages.ToRecursive(IDPropertyName: nameof(TreeNodeItem.NodeID), nameof(TreeNodeItem.ParentID), nameof(TreeNodeItem.Children));
            }

            return viewModel;
        }

        public async Task<AjaxResponse> CreatePage(SubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();

            var allowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Pages.Update);
            var allowAddNew = User.HasPermission(ControllerActionRouteNames.Admin.Pages.AddNew);
            var allowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Pages.Delete);

            TreeNodeItem node = null;

            var repository = RepositoriesFactory.GetPagesRepository();
            var pageID = await repository.PagesIUD(
                databaseAction: Enums.DatabaseActions.CREATE,
                pageParentID: submitModel.PageParentID,
                pageSlug: System.Guid.NewGuid().ToString(),
                pageTitle: submitModel.PageTitle,
                pageIsMenuItem: false,
                pageIsPublished: false,
                pageIsFooterItem: false
            );

            if (pageID > 0)
            {
                var DBItem = await repository.PagesGetSingleByID(pageID);
                node = new TreeNodeItem();
                if (DBItem != null)
                {
                    node.NodeID = pageID.ToString();
                    node.ParentID = submitModel.PageParentID.HasValue ? submitModel.PageParentID.ToString() : null;
                    node.Caption = DBItem.PageTitle;
                    node.ShowToggler1 = allowUpdate;
                    node.ShowToggler2 = allowUpdate;
                    node.ShowToggler3 = allowUpdate;
                    node.TextToggler1 = Resources.TextPublished;
                    node.TextToggler2 = Resources.TextMenu;
                    node.TextToggler3 = Resources.TextFooter;
                    node.NavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.Properties, new { PageID = pageID });
                    node.ShowAddNewButton = allowAddNew;
                    node.ShowDeleteButton = allowDelete;
                }
            }

            if (node != null)
            {
                viewModel.IsSuccess = true;
                viewModel.Data = node;
            }

            return viewModel;
        }

        public async Task<AjaxResponse> DeleteRecursive(int? pageID)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.GetPagesRepository();
            await repository.PagesDeleteRecursive(pageID);
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }

        public async Task<AjaxResponse> UpdatePage(SubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.GetPagesRepository();
            await repository.PagesIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                pageID: submitModel.PageID,
                pageTitle: submitModel.PageTitle,
                pageIsPublished: submitModel.PageIsPublished,
                pageIsMenuItem: submitModel.PageIsMenuItem,
                pageIsFooterItem: submitModel.PageIsFooterItem
            );
            viewModel.IsSuccess = !repository.IsError;

            return viewModel;
        }

        public async Task<AjaxResponse> SyncParentsAndSortIndexes(SyncSortIndexesSubmitModel SubmitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.GetPagesRepository();
            await repository.PagesSyncParentsAndSortIndexes(SubmitModel.SortIndexes);
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }
        #endregion

        #region Nested Classes
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

        public class SubmitModel
        {
            #region Properties
            public int? PageID { get; set; }
            public int? PageParentID { get; set; }
            public string PageTitle { get; set; }
            public bool? PageIsPublished { get; set; }
            public bool? PageIsMenuItem { get; set; }
            public bool? PageIsFooterItem { get; set; }
            #endregion
        }
        #endregion
    }

    public class PageModelBase : ModelBase
    {
        #region Properties
        public PageDTO DBItem { get; set; }
        #endregion
    }

    public class PagePropertiesModel : PageModelBase
    {
        #region Properties
        readonly string _folderPath = FileStorageManager.Modules[Enums.FileManagerModules.Pages].FolderName;
        #endregion

        #region Methods
        public PageViewModel GetPageViewModel(PageViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new PageViewModel();
                viewModel.PageIsPublished = DBItem.PageIsPublished;
                viewModel.PageIsMenuItem = DBItem.PageIsMenuItem;
                viewModel.PageIsFooterItem = DBItem.PageIsFooterItem;
                viewModel.PageIsExternalUrl = DBItem.PageIsExternalUrl;
                viewModel.PageExternalUrl = DBItem.PageExternalUrl;
                viewModel.PageSlug = DBItem.PageSlug;
                viewModel.PageTitle = DBItem.PageTitle;
                viewModel.PageTitleEng = DBItem.PageTitleEng;
                viewModel.PageShortDescription = DBItem.PageShortDescription;
                viewModel.PageShortDescriptionEng = DBItem.PageShortDescriptionEng;
            }

            viewModel.PageImageFilename = DBItem.PageImageFilename;
            viewModel.PageImageHttpPath = FileStorage.GetUploadedFileHttpPath(DBItem.PageImageFilename, _folderPath);

            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.DeleteImage, values: new { pageID = DBItem.PageID });

            return viewModel;
        }

        public void ValidatePageViewModel(PageViewModel viewModel)
        {
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.PageTitle)), valueToValidate: viewModel.PageTitle));
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.PageSlug)), valueToValidate: viewModel.PageSlug));
        }

        public async Task Save(PageViewModel viewModel)
        {
            var hasPageImage = viewModel.PageImageFile?.Length > 0;
            var pageImageFilename = hasPageImage ? GetFilenameFromUploadedFile(viewModel.PageImageFile) : null;
            if (hasPageImage)
            {
                await DeleteUploadedFile(pageImageFilename, _folderPath);
            }

            var repository = RepositoriesFactory.GetPagesRepository();
            await repository.PagesIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                pageID: DBItem.PageID,
                pageSlug: viewModel.PageSlug,
                pageTitle: viewModel.PageTitle,
                pageTitleEng: viewModel.PageTitleEng,
                pageShortDescription: viewModel.PageShortDescription,
                pageShortDescriptionEng: viewModel.PageShortDescriptionEng,
                pageImageFilename: pageImageFilename,
                pageIsPublished: viewModel.PageIsPublished,
                pageIsMenuItem: viewModel.PageIsMenuItem,
                pageIsFooterItem: viewModel.PageIsFooterItem,
                pageIsExternalUrl: viewModel.PageIsExternalUrl,
                pageExternalUrl: viewModel.PageExternalUrl
            );

            if (!repository.IsError)
            {
                viewModel.IsSaved = true;
                if (hasPageImage)
                {
                    await SaveUploadedFile(viewModel.PageImageFile, pageImageFilename, _folderPath);
                }
            }
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();

            await DeleteUploadedFile(DBItem.PageImageFilename, _folderPath);

            var repository = RepositoriesFactory.GetPagesRepository();
            await repository.PagesIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                pageID: DBItem.PageID,
                pageImageFilename: Constants.NullValueFor.String
            );
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class PageViewModel : FormViewModelBase
        {
            #region Properties             
            public string PageSlug { get; set; }
            public string PageTitle { get; set; }
            public string PageTitleEng { get; set; }
            public string PageShortDescription { get; set; }
            public string PageShortDescriptionEng { get; set; }
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

            public readonly string TextGenerateFromTitle = Resources.TextGenerateFromTitle;
            public readonly string TextPublished = Resources.TextPublished;
            public readonly string TextMenu = Resources.TextMenu;
            public readonly string TextFooter = Resources.TextFooter;
            public readonly string TextUploadImage = Resources.TextUploadImage;
            public readonly string TextTitle = Resources.TextTitle;
            public readonly string TextTitleEng = Resources.TextTitleEng;
            public readonly string TextSlug = Resources.TextSlug;
            public readonly string TextPageUrl = Resources.TextPageUrl;
            public readonly string TextExternalPage = Resources.TextExternalPage;
            public readonly string TextDescriptionShort = Resources.TextDescriptionShort;
            public readonly string TextDescriptionShortEng = Resources.TextDescriptionShortEng;
            #endregion
        }
        #endregion
    }

    public class PageBuilderModel : PageModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel(int? pageID, string languageCultureCode)
        {
            if (string.IsNullOrWhiteSpace(languageCultureCode))
            {
                languageCultureCode = Enums.Languages.GEORGIAN;
            }

            var viewModel = new PageViewModel();
            viewModel.PageTitle = Utilities.GetValuesByLanguage(languageCultureCode, DBItem.PageTitle, DBItem.PageTitleEng);
            viewModel.PageSlug = DBItem.PageSlug;
            viewModel.PageText = Utilities.GetValuesByLanguage(languageCultureCode, DBItem.PageText, DBItem.PageTextEng);
            viewModel.PageData = Utilities.GetValuesByLanguage(languageCultureCode, DBItem.PageData, DBItem.PageDataEng) ?? "[]";
            viewModel.IsPublished = DBItem.PageIsPublished;
            viewModel.Language = languageCultureCode;
            viewModel.UrlBack = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.Properties, new { pageID = DBItem.PageID });
            viewModel.UrlPreview = Url.RouteUrl(ControllerActionRouteNames.Website.Pages.Page, new { pageSlugHierarchy = DBItem.PageSlugHierarchy });
            viewModel.UrlSave = UrlCurrentPageWithDomain;
            viewModel.UrlFileManager = Url.RouteUrl(ControllerActionRouteNames.Admin.FileManager.Page, new { moduleName = Enums.FileManagerModules.Pages });
            viewModel.UrlPdfViewer = Url.RouteUrl(ControllerActionRouteNames.Website.FileViewer.Pdf);

            viewModel.SelectedLanguage = Utilities.GetValuesByLanguage(languageCultureCode, Enums.Languages.GEORGIAN, Enums.Languages.ENGLISH);
            viewModel.LanguageOptions =
            [
                new() { Key = nameof(Enums.Languages.GEORGIAN), Value = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.BuilderLanguage, new { pageID, Language = Enums.Languages.GEORGIAN }), IsSelected = languageCultureCode == Enums.Languages.GEORGIAN },
                new() { Key = nameof(Enums.Languages.ENGLISH), Value = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.BuilderLanguage, new { pageID, Language = Enums.Languages.ENGLISH }), IsSelected = languageCultureCode == Enums.Languages.ENGLISH },
            ];

            viewModel.PluginsClient = new PluginsClient();
            return viewModel;
        }

        public Errors ValidatePageViewModel(SubmitModel submitModel)
        {
            var errors = new Errors();
            errors.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(submitModel.PageTitle)), valueToValidate: submitModel.PageTitle));
            return errors;
        }

        public async Task<AjaxResponse> Save(SubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.GetPagesRepository();

            switch (submitModel.Language)
            {
                case Enums.Languages.GEORGIAN:
                    {
                        await repository.PagesIUD(
                            databaseAction: Enums.DatabaseActions.UPDATE,
                            pageID: DBItem.PageID,
                            pageTitle: submitModel.PageTitle ?? Constants.NullValueFor.String,
                            pageText: submitModel.PageText ?? Constants.NullValueFor.String,
                            pageTextHeaderHtml: submitModel.HeaderSectionHtml ?? Constants.NullValueFor.String,
                            pageTextFooterHtml: submitModel.FooterSectionHtml ?? Constants.NullValueFor.String,
                            pageData: submitModel.PageData ?? Constants.NullValueFor.String
                        );
                        break;
                    }
                case Enums.Languages.ENGLISH:
                    {
                        await repository.PagesIUD(
                            databaseAction: Enums.DatabaseActions.UPDATE,
                            pageID: DBItem.PageID,
                            pageTitleEng: submitModel.PageTitle ?? Constants.NullValueFor.String,
                            pageTextEng: submitModel.PageText ?? Constants.NullValueFor.String,
                            pageTextHeaderHtmlEng: submitModel.HeaderSectionHtml ?? Constants.NullValueFor.String,
                            pageTextFooterHtmlEng: submitModel.FooterSectionHtml ?? Constants.NullValueFor.String,
                            pageDataEng: submitModel.PageData ?? Constants.NullValueFor.String
                        );
                        break;
                    }
            }
            viewModel.IsSuccess = !repository.IsError;

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class PageViewModel
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
            public List<KeyValueSelectedTuple<string, string>> LanguageOptions { get; set; }
            public bool HasLanguageOptions => LanguageOptions?.Count > 0;

            public readonly string FileManagerAllowedExtensions = WebConstants.QueryStringKeys.FileManagerAllowedExtensions;
            public readonly string FileManagerAllowChooseMultiple = WebConstants.QueryStringKeys.FileManagerAllowChooseMultiple;
            public readonly string FileManagerOnSelectedFilesChooseClientCallback = WebConstants.QueryStringKeys.FileManagerOnSelectedFilesChooseClientCallback;
            public readonly string TextError = Resources.TextError;
            #endregion
        }

        public class SubmitModel
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