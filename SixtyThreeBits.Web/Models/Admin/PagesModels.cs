using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Libraries.FileStorages;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class PagesManagementModule : ModelBase
    {
        #region Methods
        public string GetRedirectUrl()
        {
            var redirectUrl = default(string);
            var permissionIDParent = User.Permissions.FindLast(Item => Item.PermissionCodeName == ControllerActionRouteNames.Admin.PagesManagementController.RedirectToChild)?.PermissionID;
            if (permissionIDParent.HasValue)
            {
                var firstPermission = User.Permissions.FirstOrDefault(item => item.PermissionParentID == permissionIDParent);
                redirectUrl = firstPermission.PermissionPagePath;
            }
            return redirectUrl;
        } 
        #endregion
    }

    public class PagesModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.PagesController.GridAdd);
            viewModel.Grid = new ViewModel.GridModel();            
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.PagesController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.PagesController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.PagesController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.PagesController.GridDelete);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.PagesController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.PagesController.GridDelete);
            return viewModel;
        }

        public async Task<List<ViewModel.GridModel.GridItem>> GetGridModel()
        {
            var repository = RepositoriesFactory.CreatePagesRepository();

            var viewModel = (await repository.PagesList())
            ?.Select(item => new ViewModel.GridModel.GridItem
            {
                PageID = item.PageID,
                PageTitle = item.PageTitle,
                PageTitleEng = item.PageTitleEng,
                PageIsPublished = item.PageIsPublished,
                PageDateCreated = item.PageDateCreated,
                UrlProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.PagePropertiesController.Properties, new { pageID = item.PageID })
            })
            .ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> IUD(Enums.DatabaseActions databaseAction, int? pageID, ViewModel.GridModel.GridItem submitModel)
        {
            var viewModel = new AjaxResponse();

            var repository = RepositoriesFactory.CreatePagesRepository();
            await repository.PagesIUD(
                databaseAction: databaseAction,
                pageID: pageID,
                page: new PageIudDTO
                {
                    PageTitle = submitModel.PageTitle,
                    PageTitleEng = submitModel.PageTitleEng,
                    PageIsPublished = submitModel.PageIsPublished
                }
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }

        public async Task<AjaxResponse> Delete(int? pageID)
        {
            var viewModel = new AjaxResponse();

            await deleteProcessPageImageFilename(pageID);

            var repository = RepositoriesFactory.CreatePagesRepository();
            await repository.PagesDelete(pageID: pageID);
            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }
        async Task deleteProcessPageImageFilename(int? pageID)
        {
            var repository = RepositoriesFactory.CreatePagesRepository();
            var page = await repository.PagesGetSingleByID(pageID);
            await FileStorage.DeleteFile(page.PageImageFilename);
        }

        public async Task<AjaxResponse> GetPagesData()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreatePagesRepository();

            var pages = (await repository.PagesList())?
            .OrderBy(item => item.PageTitle)
            .Select(item => new KeyValueTuple<int?, string>
            {
                Key = item.PageID,
                Value = $"{string.Join(" | ", item.PageTitle, item.PageTitleEng)}"
            })
            .ToList();

            viewModel.Data = pages;
            viewModel.IsSuccess = true;

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridViewModelBase<GridModel.GridItem>
            {
                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.PageID));

                    grid
                    .ID("PagesGrid")
                    .OnInitialized("model.onGridInit")
                    .Columns(columns =>
                    {
                        columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlProperties));
                        columns.AddFor(m => m.PageTitle).Caption(Resources.TextTitle).Width(300).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.AddFor(m => m.PageTitleEng).Caption(Resources.TextTitleEng).Width(300);                        
                        columns.AddFor(m => m.PageIsPublished).Caption(Resources.TextPublished).Width(100).InitCheckboxColumn();
                        columns.AddFor(m => m.PageDateCreated).Caption(Resources.TextDateCreated).DataType(GridColumnDataType.DateTime).Width(140).InitDateColumn(true).AllowEditing(false);
                        columns.Add();
                    });


                    return grid;
                }
                #endregion

                #region Nested Classes
                public record GridItem
                {
                    #region Properties
                    public int? PageID { get; init; }
                    public string PageSlug { get; init; }
                    public string PageTitle { get; init; }
                    public string PageTitleEng { get; init; }                    
                    public bool? PageIsPublished { get; init; }
                    public DateTime? PageDateCreated { get; init; }
                    public string UrlProperties { get; init; }                    
                    #endregion
                }
                #endregion
            }
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

    public class PageDataModel : PageModelBase
    {
        #region Methods
        public AjaxResponse GetPageData()
        {
            var viewModel = new AjaxResponse();
            viewModel.IsSuccess = true;
            viewModel.Data = new
            {
                PageID = DBItem.PageID,
                PageTitle = DBItem.PageTitle,
                PageTitleEng = DBItem.PageTitleEng,
                PageSlug = DBItem.PageSlug,
                PageIsPublished = DBItem.PageIsPublished
            };
            return viewModel;
        }
        #endregion
    }

    public class PagePropertiesModel : PageModelBase
    {
        #region Properties
        readonly string _folderPath = FileStorageManager.Modules[Enums.FileManagerModules.Pages].FolderName;
        #endregion

        #region Methods
        public ViewModel GetViewModel(ViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
                viewModel.PageIsPublished = DBItem.PageIsPublished;
                viewModel.PageSlug = DBItem.PageSlug;
                viewModel.PageTitle = DBItem.PageTitle;
                viewModel.PageTitleEng = DBItem.PageTitleEng;
                viewModel.PageShortDescription = DBItem.PageShortDescription;
                viewModel.PageShortDescriptionEng = DBItem.PageShortDescriptionEng;
            }

            viewModel.PageImageFilename = DBItem.PageImageFilename;
            viewModel.PageImageHttpPath = FileStorage.GetUploadedFileHttpPath(DBItem.PageImageFilename, _folderPath);

            viewModel.UrlPreview = GetUrlPages(pageSlug:DBItem.PageSlug);
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.PagePropertiesController.DeleteImage, values: new { pageID = DBItem.PageID });

            return viewModel;
        }

        public async Task ValidateViewModel(ViewModel viewModel)
        {
            viewModel.AddError(Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.PageTitle)), valueToValidate: viewModel.PageTitle));
            viewModel.AddError(Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.PageSlug)), valueToValidate: viewModel.PageSlug));
            viewModel.AddError(
                await Validation63.ValidateAsync(
                        errorAction: async () =>
                        {
                            var repository = RepositoriesFactory.CreatePagesRepository();
                            var isUniq = await repository.PagesIsSlugUniq(pageSlug: viewModel.PageSlug, pageID: DBItem.PageID);
                            var isError = !isUniq;
                            return isError;
                        },
                        errorKey: Validation63.GetJQueryNameSelectorFor(nameof(viewModel.PageSlug)),
                        errorMessage: Resources.ValidationPagesSlugNotUniq
                    )
            );
        }

        public async Task Save(ViewModel viewModel)
        {
            var hasPageImage = viewModel.PageImageFile?.Length > 0;
            var pageImageFilename = hasPageImage ? GetFilenameFromUploadedFile(viewModel.PageImageFile) : null;
            if (hasPageImage)
            {
                await FileStorage.DeleteFile(pageImageFilename, _folderPath);
            }

            var repository = RepositoriesFactory.CreatePagesRepository();
            await repository.PagesIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                pageID: DBItem.PageID,
                page: new PageIudDTO
                {
                    PageSlug = viewModel.PageSlug,
                    PageTitle = viewModel.PageTitle,
                    PageTitleEng = viewModel.PageTitleEng ?? Constants.NullValueFor.String,
                    PageShortDescription = viewModel.PageShortDescription ?? Constants.NullValueFor.String,
                    PageShortDescriptionEng = viewModel.PageShortDescriptionEng ?? Constants.NullValueFor.String,
                    PageImageFilename = pageImageFilename,
                    PageIsPublished = viewModel.PageIsPublished
                }
            );

            if (repository.IsError)
            {
                viewModel.AddError(repository.ErrorMessage);
            }
            else
            {
                if (hasPageImage)
                {
                    await SaveUploadedFile(viewModel.PageImageFile, pageImageFilename, _folderPath);
                }
            }
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();

            await FileStorage.DeleteFile(DBItem.PageImageFilename, _folderPath);

            var repository = RepositoriesFactory.CreatePagesRepository();
            await repository.PagesIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                pageID: DBItem.PageID,
                page: new PageIudDTO
                {
                    PageImageFilename = Constants.NullValueFor.String
                }
            );
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel : FormViewModelBase
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
            public IFormFile PageImageFile { get; set; }
            public string UrlDeleteImage { get; set; }
            public string UrlPreview { get; set; }

            public readonly string TextPreview = Resources.TextPreview;
            public readonly string TextGenerateFromTitle = Resources.TextGenerateFromTitle;
            public readonly string TextPublished = Resources.TextPublished;
            public readonly string TextUploadImage = Resources.TextUploadImage;
            public readonly string TextTitle = Resources.TextTitle;
            public readonly string TextTitleEng = Resources.TextTitleEng;            
            public readonly string TextSlug = Resources.TextSlug;            
            public readonly string TextPageUrl = Resources.TextPageUrl;
            public readonly string TextDescriptionShort = Resources.TextDescriptionShort;
            public readonly string TextDescriptionShortEng = Resources.TextDescriptionShortEng;
            public readonly string TextPageShortDescriptionAndImageInfo = Resources.TextPageShortDescriptionAndImageInfo;
            #endregion
        }
        #endregion
    }

    public class PageBuilderModel : PageModelBase
    {
        #region Methods
        public ViewModel GetViewModel(int? pageID, string languageCultureCode)
        {
            if (string.IsNullOrWhiteSpace(languageCultureCode))
            {
                languageCultureCode = Enums.Languages.GEORGIAN;
            }

            var viewModel = new ViewModel();
            viewModel.PageTitle = Utilities.GetValuesByLanguage(languageCultureCode, DBItem.PageTitle, DBItem.PageTitleEng);
            viewModel.PageText = Utilities.GetValuesByLanguage(languageCultureCode, DBItem.PageText, DBItem.PageTextEng);
            viewModel.PageData = Utilities.GetValuesByLanguage(languageCultureCode, DBItem.PageData, DBItem.PageDataEng) ?? "[]";
            viewModel.IsPublished = DBItem.PageIsPublished;
            viewModel.Language = languageCultureCode;
            viewModel.UrlBack = Url.RouteUrl(ControllerActionRouteNames.Admin.PagePropertiesController.Properties, new { pageID = DBItem.PageID });
            viewModel.UrlPreview = GetRouteByName(ControllerActionRouteNames.Website.PagesController.Page, new { pageSlug = DBItem.PageSlug });
            viewModel.UrlSave = UrlCurrentPageWithDomain;
            viewModel.UrlFileManager = Url.RouteUrl(ControllerActionRouteNames.Admin.FileManagerController.FileManager, new { moduleName = Enums.FileManagerModules.Pages });

            viewModel.SelectedLanguage = Utilities.GetValuesByLanguage(languageCultureCode, Enums.Languages.GEORGIAN, Enums.Languages.ENGLISH);
            viewModel.LanguageOptions =
            [
                new() { Key = nameof(Enums.Languages.GEORGIAN), Value = Url.RouteUrl(ControllerActionRouteNames.Admin.PageBuilderController.BuilderLanguage, new { pageID, Language = Enums.Languages.GEORGIAN }), IsSelected = languageCultureCode == Enums.Languages.GEORGIAN },
                new() { Key = nameof(Enums.Languages.ENGLISH), Value = Url.RouteUrl(ControllerActionRouteNames.Admin.PageBuilderController.BuilderLanguage, new { pageID, Language = Enums.Languages.ENGLISH }), IsSelected = languageCultureCode == Enums.Languages.ENGLISH },                
            ];

            viewModel.PluginsClient = new PluginsClientViewModel();
            return viewModel;
        }

        public ValidationResult63 Validate(SubmitModel submitModel)
        {
            var validationResult = new ValidationResult63();
            validationResult.AddError(Validation63.ValidateRequired(errorKey: Validation63.GetJQueryNameSelectorFor(nameof(submitModel.PageTitle)), valueToValidate: submitModel.PageTitle));
            return validationResult;
        }

        public async Task<AjaxResponse> Save(SubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreatePagesRepository();

            switch (submitModel.Language)
            {
                case Enums.Languages.GEORGIAN:
                    {
                        await repository.PagesIUD(
                            databaseAction: Enums.DatabaseActions.UPDATE,
                            pageID: DBItem.PageID,
                            page: new PageIudDTO
                            {
                                PageText = submitModel.PageText ?? Constants.NullValueFor.String,
                                PageTextHeaderHtml = submitModel.HeaderSectionHtml ?? Constants.NullValueFor.String,
                                PageTextFooterHtml = submitModel.FooterSectionHtml ?? Constants.NullValueFor.String,
                                PageData = submitModel.PageData ?? Constants.NullValueFor.String
                            }
                        );
                        break;
                    }
                case Enums.Languages.ENGLISH:
                    {
                        await repository.PagesIUD(
                            databaseAction: Enums.DatabaseActions.UPDATE,
                            pageID: DBItem.PageID,
                            page: new PageIudDTO
                            {
                                PageTextEng = submitModel.PageText ?? Constants.NullValueFor.String,
                                PageTextHeaderHtmlEng = submitModel.HeaderSectionHtml ?? Constants.NullValueFor.String,
                                PageTextFooterHtmlEng = submitModel.FooterSectionHtml ?? Constants.NullValueFor.String,
                                PageDataEng = submitModel.PageData ?? Constants.NullValueFor.String
                            }
                            
                        );
                        break;
                    }                
            }
            viewModel.IsSuccess = !repository.IsError;

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public PluginsClientViewModel PluginsClient { get; set; }
            public string PageTitle { get; set; }
            public string PageText { get; set; }
            public bool IsPublished { get; set; }
            public string Language { get; set; }
            public string PageData { get; set; }
            public string UrlBack { get; set; }
            public string UrlPreview { get; set; }
            public string UrlSave { get; set; }
            public string UrlFileManager { get; set; }            
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