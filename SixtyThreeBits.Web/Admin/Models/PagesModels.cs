using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
                ShowAddNewButton = AllowAddNew,
                ShowDeleteButton = AllowDelete,
                ShowToggler1 = AllowUpdate,
                ShowToggler2 = AllowUpdate
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
                PageIsPublished: false                
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

        public async Task<AjaxResponse> UpdatePage(int? PageID, string PageTitle = null, bool? PageIsPublished = null, bool? PageIsMenuItem = null)
        {
            var AR = new AjaxResponse();
            await DataAccessFactory.Pages.PagesIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                PageID: PageID,
                PageTitle: PageTitle,
                PageIsPublished: PageIsPublished,
                PageIsMenuItem: PageIsMenuItem
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
            public string TextConfirmDeleteRecord { get; set; } = Resources.TextConfirmDelete;
            public string TextConfirmDeleteRecursive { get; set; } = Resources.TextConfirmDeleteRecursive;
            public string ValidationRequiredPageTitle { get; set; } = Resources.ValidationRequiredPageTitle;
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
            ViewModel.PageImageHttpPath = DBItemPage.PageImageFilenameHttpPath;

            return ViewModel;
        }

        public async Task ValidatePagePropertiesViewModel(PagePropertiesViewModel ViewModel)
        {
            ViewModel.Errors = new List<SimpleKeyValue<string, string>>
            {
                Validation.ValidateRequired(ErrorKey:$"[name=\"{nameof(ViewModel.PageTitle)}\"]", ValueToValidate:ViewModel.PageTitle),
                Validation.ValidateRequired(ErrorKey:$"[name=\"{nameof(ViewModel.PageSlug)}\"]", ValueToValidate:ViewModel.PageSlug),
                await Validation.ValidateAsync(
                    ErrorAction: async () =>
                    {
                        var IsUniq = await DataAccessFactory.Pages.IsPageSlugUniq(PageSlug:ViewModel.PageSlug,PageID:DBItemPage.PageID);
                        return !IsUniq;
                    },
                    ErrorKey: $"[name=\"{nameof(ViewModel.PageSlug)}\"]",
                    ErrorMessage: Resources.ValidationPageSlugNotUniq
                )
            };
            ViewModel.Errors.RemoveAll(Item => Item == null);
        }

        public async Task<bool> SavePageProperties(PagePropertiesViewModel ViewModel)
        {
            var HasPageImage = ViewModel.PageImageFile?.Length > 0;
            var PageImageFilename = HasPageImage ? GetFilenameFromUploadedFile(ViewModel.PageImageFile) : null;
            if (HasPageImage)
            {
                Utilities.DeleteUploadedFile(DBItemPage.PageImageFilename, DBItemPage.FolderPhysicalPath);
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
                    await SaveUploadedFile(PostedFile: ViewModel.PageImageFile, Filename: PageImageFilename, FolderPhysicalPath: DBItemPage.FolderPhysicalPath);
                }
            }

            return ViewModel.IsSaved;
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
            ViewModel.UrlPreview = GetRouteByName(ControllerActionRouteNames.Website.Home.StaticPagePreview, new { Culture = Language, PageID });
            ViewModel.UrlSave = UrlCurrentPage;
            ViewModel.UrlFileManager = GetFileManagerUrl(DBItemPage.FolderPhysicalPath, DBItemPage.FolderVirtualPath);

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

        public async Task<List<SimpleKeyValue<string, string>>> ValidatePageBuilderSubmitModel(PageBuilderSubmitModel SubmitModel)
        {
            var Errors = new List<SimpleKeyValue<string, string>>
            {
                Validation.ValidateRequired(ErrorKey: $"[name=\"{nameof(SubmitModel.PageTitle)}\"]", ValueToValidate: SubmitModel.PageTitle),
                Validation.ValidateRequired(ErrorKey: $"[name=\"{nameof(SubmitModel.PageSlug)}\"]", ValueToValidate: SubmitModel.PageSlug),
                await Validation.ValidateAsync(
                    ErrorAction: async () =>
                    {
                        var IsUniq = await DataAccessFactory.Pages.IsPageSlugUniq(PageSlug:SubmitModel.PageSlug, PageID:DBItemPage.PageID);
                        return !IsUniq;
                    },
                    ErrorKey: $"[name=\"{nameof(SubmitModel.PageSlug)}\"]",
                    ErrorMessage: Resources.ValidationPageSlugNotUniq
                )
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
                            PageSlug: SubmitModel.PageSlug,
                            PageTitle: SubmitModel.PageTitle ?? Constants.NullValueFor.String,
                            PageText: SubmitModel.PageText ?? Constants.NullValueFor.String,
                            PageData: SubmitModel.PageData ?? Constants.NullValueFor.String,
                            PageIsPublished: SubmitModel.IsPublished
                        );
                        break;
                    }
                case Enums.Languages.ENGLISH:
                    {
                        await DataAccessFactory.Pages.PagesIUD(
                            DatabaseAction: Enums.DatabaseActions.UPDATE,
                            PageID: DBItemPage.PageID,
                            PageSlug: SubmitModel.PageSlug,
                            PageTitleEng: SubmitModel.PageTitle ?? Constants.NullValueFor.String,
                            PageTextEng: SubmitModel.PageText ?? Constants.NullValueFor.String,
                            PageDataEng: SubmitModel.PageData ?? Constants.NullValueFor.String,
                            PageIsPublished: SubmitModel.IsPublished
                        );
                        break;
                    }
                case Enums.Languages.RUSSIAN:
                    {
                        await DataAccessFactory.Pages.PagesIUD(
                            DatabaseAction: Enums.DatabaseActions.UPDATE,
                            PageID: DBItemPage.PageID,
                            PageSlug: SubmitModel.PageSlug,
                            PageTitleRus: SubmitModel.PageTitle ?? Constants.NullValueFor.String,
                            PageTextRus: SubmitModel.PageText ?? Constants.NullValueFor.String,
                            PageDataRus: SubmitModel.PageData ?? Constants.NullValueFor.String,
                            PageIsPublished: SubmitModel.IsPublished
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
            public bool IsPublished { get; set; }
            #endregion
        }
        #endregion
    }
    //public class PagePropertiesModel
    //{
    //    #region Methods
    //    public static AjaxResponse DeleteImage(int? PageID)
    //    {
    //        var DBItem = PagesDataAccess.GetSinglePageByID(PageID);
    //        Utility.DeleteUploadedFile(DBItem.PageImageFilename, DBItem.FolderPhysicalPath);

    //        var AR = new AjaxResponse();
    //        var P = new PagesDataAccess();
    //        P.PagesIUD(
    //            DatabaseAction: Enums.DatabaseActions.UPDATE,
    //            PageID: PageID,
    //            PageImageFilename: Constants.NullValueFor.String
    //            );

    //        AR.IsSuccess = !P.IsError;

    //        return AR;
    //    }

    //    public static PageViewModel GetPageViewModel(Page DBItem, PageViewModel Model, UrlHelper Url)
    //    {
    //        if (Model == null)
    //        {
    //            Model = new PageViewModel
    //            {
    //                PageID = DBItem.PageID,
    //                PageSlug = DBItem.PageSlug,
    //                PageIsPublished = DBItem.PageIsPublished,
    //                PageIsMenuItem = DBItem.PageIsMenuItem,
    //                PageTitle = DBItem.PageTitle,
    //                PageTitleEng = DBItem.PageTitleEng,
    //                PageTitleRus = DBItem.PageTitleRus,                    
    //                PageShortDescription = DBItem.PageShortDescription,
    //                PageShortDescriptionEng = DBItem.PageShortDescriptionEng,
    //                PageShortDescriptionRus = DBItem.PageShortDescriptionRus,                    
    //                PageImageFilename = DBItem.PageImageFilename                    
    //            };
    //        }

    //        Model.UrlFileManager = LocalUtilities.GetFileManagerUrl(Url, DBItem.FolderPhysicalPath, DBItem.FolderVirtualPath);
    //        Model.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.DeleteImage);

    //        return Model;
    //    }

    //    public static PageViewModel SavePageItem(int? PageID, PageViewModel Model, int? EditorUserID)
    //    {
    //        var Page = PagesDataAccess.GetSinglePageByID(PageID);
    //        var NewFilename = Utility.GetFilenameFromUploadedFile(Model.PostedFile);
    //        var IsPageImageUpdating = Model.PostedFile != null;
    //        if (IsPageImageUpdating)
    //        {
    //            Utility.DeleteUploadedFile(Page.PageImageFilename, Page.FolderPhysicalPath);
    //        }

    //        var DAL = new PagesDataAccess();
    //        DAL.PagesIUD(
    //            DatabaseAction: Enums.DatabaseActions.UPDATE,
    //            PageID: PageID,
    //            PageSlug: Model.PageSlug,
    //            PageIsPublished: Model.PageIsPublished,
    //            PageIsMenuItem: Model.PageIsMenuItem,
    //            PageTitle: Model.PageTitle,
    //            PageTitleEng: Model.PageTitleEng ?? Constants.NullValueFor.String,
    //            PageTitleRus: Model.PageTitleRus ?? Constants.NullValueFor.String,                
    //            PageShortDescription: Model.PageShortDescription ?? Constants.NullValueFor.String,
    //            PageShortDescriptionEng: Model.PageShortDescriptionEng ?? Constants.NullValueFor.String,
    //            PageShortDescriptionRus: Model.PageShortDescriptionRus ?? Constants.NullValueFor.String,                
    //            PageImageFilename: NewFilename
    //        );

    //        Model.Form.IsSaved = !DAL.IsError;

    //        if (IsPageImageUpdating)
    //        {
    //            Utility.SaveUploadedFile(Model.PostedFile, NewFilename, Page.FolderPhysicalPath);
    //        }

    //        return Model;
    //    }

    //    public static void ValidatePageItemViewModel(int? PageID, PageViewModel Model)
    //    {
    //        Model.Form.Errors = new List<SimpleKeyValue<string, string>>();

    //        Model.Form.Errors.Add(Validation.ValidateRequired(ErrorKey: $"[name={nameof(Model.PageTitle)}]", ValueToValidate: Model.PageTitle));

    //        if (string.IsNullOrWhiteSpace(Model.PageSlug))
    //        {
    //            Model.Form.Errors.Add(Validation.GetError(ErrorKey: $"[name=\"{nameof(Model.PageSlug)}\"]", ErrorMessage: Resources.ValidationRequiredField));
    //        }
    //        else if (!PagesDataAccess.IsPageSlugUniq(Model.PageSlug, PageID))
    //        {
    //            Model.Form.Errors.Add(Validation.GetError(ErrorKey: $"[name=\"{nameof(Model.PageSlug)}\"]", ErrorMessage: Resources.TextSlugNotUniq));
    //        }

    //        Model.Form.Errors.RemoveAll(Item => Item == null);
    //    }
    //    #endregion

    //    #region Sub Classes
    //    public class PageViewModel : Page
    //    {
    //        #region Properties                        
    //        public FormViewModelBase Form { get; set; } = new FormViewModelBase();
    //        public HttpPostedFileBase PostedFile { get; set; }
    //        public string UrlFileManager { get; set; }
    //        public string UrlDeleteImage { get; set; }
    //        public string TextConfirmDelete { get; set; } = Resources.TextConfirmDelete;
    //        #endregion
    //    }
    //    #endregion
    //}

    //public class PageBuilderModel
    //{
    //    #region Methods
    //    public static PageViewModel GetPageViewModel(int? PageID, string Language, PageController C)
    //    {
    //        if (string.IsNullOrWhiteSpace(Language))
    //        {
    //            Language = Enums.Languages.GEORGIAN;
    //        }

    //        var Model = new PageViewModel();
    //        Model.PageTitle = Utility.GetValuesByLanguage(Language, C.DBItemPage.PageTitle, C.DBItemPage.PageTitleEng, C.DBItemPage.PageTitleRus);
    //        Model.PageSlug = C.DBItemPage.PageSlug;
    //        Model.PageText = Utility.GetValuesByLanguage(Language, C.DBItemPage.PageText, C.DBItemPage.PageTextEng, C.DBItemPage.PageTextRus);
    //        Model.PageData = Utility.GetValuesByLanguage(Language, C.DBItemPage.PageData, C.DBItemPage.PageDataEng, C.DBItemPage.PageDataRus) ?? "[]";
    //        Model.IsPublished = C.DBItemPage.PageIsPublished;
    //        Model.Language = Language;
    //        Model.UrlBack = C.Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Index);
    //        Model.UrlPreview = C.Url.RouteUrl(ControllerActionRouteNames.Website.Home.StaticPagePreviewCulture, new { Culture = Language, PageID });
    //        Model.UrlSave = C.Request.RawUrl;
    //        Model.UrlFileManager = LocalUtilities.GetFileManagerUrl(C.Url, C.DBItemPage.FolderPhysicalPath, C.DBItemPage.FolderVirtualPath);            

    //        Model.SelectedLanguage = Utility.GetValuesByLanguage(Language, Enums.Languages.GEORGIAN, Enums.Languages.ENGLISH, Enums.Languages.RUSSIAN);
    //        Model.LanguageOptions = new List<SimpleKeyValue<string, string>>
    //        {
    //            new SimpleKeyValue<string, string>{ Key = nameof(Enums.Languages.GEORGIAN), Value = C.Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.BuilderLanguage, new { PageID, Language = Enums.Languages.GEORGIAN}), IsSelected = Language == Enums.Languages.GEORGIAN },
    //            new SimpleKeyValue<string, string>{ Key = nameof(Enums.Languages.ENGLISH), Value = C.Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.BuilderLanguage, new { PageID, Language = Enums.Languages.ENGLISH}), IsSelected = Language == Enums.Languages.ENGLISH},
    //            new SimpleKeyValue<string, string>{ Key = nameof(Enums.Languages.RUSSIAN), Value = C.Url.RouteUrl(ControllerActionRouteNames.Admin.Pages.Page.BuilderLanguage, new { PageID, Language = Enums.Languages.RUSSIAN}), IsSelected = Language == Enums.Languages.RUSSIAN}
    //        };

    //        return Model;
    //    }

    //    public static AjaxResponse Save(int? PageID, PageSubmitModel SubmitModel)
    //    {
    //        var AR = new AjaxResponse();
    //        var DAL = new PagesDataAccess();

    //        switch (SubmitModel.Language)
    //        {
    //            case Enums.Languages.GEORGIAN:
    //                {
    //                    DAL.PagesIUD(
    //                        DatabaseAction: Enums.DatabaseActions.UPDATE,
    //                        PageID: PageID,
    //                        PageSlug: SubmitModel.PageSlug,
    //                        PageTitle: SubmitModel.PageTitle ?? Constants.NullValueFor.String,
    //                        PageText: SubmitModel.PageText ?? Constants.NullValueFor.String,
    //                        PageData: SubmitModel.PageData ?? Constants.NullValueFor.String,
    //                        PageIsPublished: SubmitModel.IsPublished
    //                    );
    //                    break;
    //                }
    //            case Enums.Languages.ENGLISH:
    //                {
    //                    DAL.PagesIUD(
    //                        DatabaseAction: Enums.DatabaseActions.UPDATE,
    //                        PageID: PageID,
    //                        PageSlug: SubmitModel.PageSlug,
    //                        PageTitleEng: SubmitModel.PageTitle ?? Constants.NullValueFor.String,
    //                        PageTextEng: SubmitModel.PageText ?? Constants.NullValueFor.String,
    //                        PageDataEng: SubmitModel.PageData ?? Constants.NullValueFor.String,
    //                        PageIsPublished: SubmitModel.IsPublished
    //                    );
    //                    break;
    //                }
    //            case Enums.Languages.RUSSIAN:
    //                {
    //                    DAL.PagesIUD(
    //                        DatabaseAction: Enums.DatabaseActions.UPDATE,
    //                        PageID: PageID,
    //                        PageSlug: SubmitModel.PageSlug,
    //                        PageTitleRus: SubmitModel.PageTitle ?? Constants.NullValueFor.String,
    //                        PageTextRus: SubmitModel.PageText ?? Constants.NullValueFor.String,
    //                        PageDataRus: SubmitModel.PageData ?? Constants.NullValueFor.String,
    //                        PageIsPublished: SubmitModel.IsPublished
    //                    );
    //                    break;
    //                }
    //        }
    //        AR.IsSuccess = !DAL.IsError;

    //        return AR;            
    //    }

    //    public static List<SimpleKeyValue<string, string>> Validate(int? PageID, PageSubmitModel SubmitModel)
    //    {
    //        var Errors = new List<SimpleKeyValue<string, string>>
    //        {
    //            Validation.ValidateRequired(ErrorKey: $"[name=\"{nameof(SubmitModel.PageTitle)}\"]", ValueToValidate: SubmitModel.PageTitle)
    //        };

    //        if (string.IsNullOrWhiteSpace(SubmitModel.PageSlug))
    //        {
    //            Errors.Add(Validation.GetError(ErrorKey: $"[name=\"{nameof(SubmitModel.PageSlug)}\"]", ErrorMessage: Resources.ValidationRequiredField));
    //        }
    //        else if (!PagesDataAccess.IsPageSlugUniq(SubmitModel.PageSlug, PageID))
    //        {
    //            Errors.Add(Validation.GetError(ErrorKey: $"[name=\"{nameof(SubmitModel.PageSlug)}\"]", ErrorMessage: Resources.TextSlugNotUniq));
    //        }

    //        Errors.RemoveAll(Item => Item == null);

    //        return Errors;
    //    }
    //    #endregion

    //    #region Sub Classes
    //    public class PageViewModel
    //    {
    //        #region Properties
    //        public string PageTitle { get; set; }
    //        public string PageSlug { get; set; }
    //        public string PageText { get; set; }
    //        public bool IsPublished { get; set; }
    //        public string Language { get; set; }
    //        public string PageData { get; set; }
    //        public string UrlBack { get; set; }
    //        public string UrlPreview { get; set; }
    //        public string UrlSave { get; set; }
    //        public string UrlFileManager { get; set; }
    //        public string SelectedLanguage { get; set; }
    //        public List<SimpleKeyValue<string,string>> LanguageOptions { get; set; }
    //        public bool HasLanguageOptions => LanguageOptions?.Count > 0;

    //        public string TextError { get; set; } = Resources.TextError;            
    //        #endregion
    //    } 

    //    public class PageSubmitModel
    //    {
    //        #region Properties
    //        public string PageTitle { get; set; }
    //        public string PageSlug { get; set; }
    //        public string PageText { get; set; }
    //        public string Language { get; set; }
    //        public string PageData { get; set; }
    //        public bool IsPublished { get; set; } 
    //        #endregion
    //    }
    //    #endregion
    //}
}