using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.Validation;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class MenuFooterModel : ModelBase
    {
        #region Properties
        bool _showAddNewButton;
        bool _showUpdateButton;
        bool _showDeleteButton;
        #endregion

        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();
            var repositoryMenuFooter = RepositoriesFactory.CreateMenuFooterRepository();
            var repositoryPages = RepositoriesFactory.CreatePagesRepository();

            _showAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuFooterController.Add);
            _showUpdateButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuFooterController.Update);
            _showDeleteButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuFooterController.Delete);

            viewModel.ShowAddNewButton = _showAddNewButton;
            viewModel.UrlAdd = Url.RouteUrl(ControllerActionRouteNames.Admin.MenuFooterController.Add);
            viewModel.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.MenuFooterController.Update);
            viewModel.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.MenuFooterController.Delete);
            viewModel.UrlSort = Url.RouteUrl(ControllerActionRouteNames.Admin.MenuFooterController.Sort);
            viewModel.UrlGet = Url.RouteUrl(ControllerActionRouteNames.Admin.MenuFooterController.Get, new { menuFooterID = 0 }).TrimEnd('0').TrimEnd('/');
            viewModel.UrlGetPage = Url.RouteUrl(ControllerActionRouteNames.Admin.PageMetaDataController.MetaData, new { pageID = 0 }).TrimEnd('0').TrimEnd('/');
            viewModel.UrlGetPages = Url.RouteUrl(ControllerActionRouteNames.Admin.PagesController.Json);

            viewModel.Pages = (await repositoryPages.PagesList())?
            .OrderBy(item => item.PageTitle)
            .Select(item => new KeyValueTuple<int?, string>
            {
                Key = item.PageID,
                Value = $"{string.Join(" | ", item.PageTitle, item.PageTitleEng)}"
            })
            .ToList();

            viewModel.MenuItems = (await repositoryMenuFooter.MenuFooterList())
            ?.Select(item => getMenuListItemFromMenuFooterDto(item))
            .ToList();
            
            return viewModel;
        }

        public async Task<AjaxResponse> Add(SubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repositoryMenuFooter = RepositoriesFactory.CreateMenuFooterRepository();
            var isError = false;

            _showAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuFooterController.Add);
            _showUpdateButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuFooterController.Update);
            _showDeleteButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuFooterController.Delete);

            var menuFooterID = await repositoryMenuFooter.MenuFooterIUD(
                databaseAction: Enums.DatabaseActions.INSERT,
                menuFooterID: null,
                menuFooter: new MenuFooterIudDTO
                {
                    MenuFooterTitle = submitModel.MenuFooterTitle,
                    MenuFooterTitleEng = submitModel.MenuFooterTitleEng,
                    PageID = submitModel.PageID,
                    MenuFooterIsExternalPage = submitModel.MenuFooterIsExternalPage,
                    MenuFooterExternalPageUrl = submitModel.MenuFooterExternalPageUrl,
                    MenuFooterIsPublished = submitModel.MenuFooterIsPublished,
                    MenuFooterIsTargetBlank = submitModel.MenuFooterIsTargetBlank
                }
            );
            isError = repositoryMenuFooter.IsError;

            if (!isError && !submitModel.MenuFooterIsExternalPage)
            {
                var repositoryPages = RepositoriesFactory.CreatePagesRepository();
                await repositoryPages.PagesIUD(
                    databaseAction: Enums.DatabaseActions.UPDATE,
                    pageID: submitModel.PageID,
                    page: new PageIudDTO
                    {
                        PageID = submitModel.PageID,
                        PageTitle = submitModel.PageTitle,
                        PageTitleEng = submitModel.PageTitleEng,
                        PageSlug = submitModel.PageSlug,
                        PageIsPublished = submitModel.PageIsPublished                        
                    }
                );
                isError = repositoryPages.IsError;
            }

            if (!isError)
            {
                viewModel.IsSuccess = true;
                var dbItem = await repositoryMenuFooter.MenuFooterGetSingleByID(menuFooterID: menuFooterID);
                if (dbItem != null)
                {
                    var partialViewModel = getMenuListItemFromMenuFooterDto(dbItem);
                    viewModel.IsSuccess = true;
                    viewModel.Data = await WebUtilities.RenderViewAsync(
                        controller: Controller,
                        contentRootPath: Utilities.ContentRootPath,
                        viewName: ViewNames.Admin.MenuFooter.MenuFooterTreeNodePartial,
                        model: partialViewModel
                    );
                }
            }
            return viewModel;
        }

        public async Task<AjaxResponse> Update(SubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repositoryMenuFooter = RepositoriesFactory.CreateMenuFooterRepository();
            var isError = false;

            _showAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuFooterController.Add);
            _showUpdateButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuFooterController.Update);
            _showDeleteButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuFooterController.Delete);

            await repositoryMenuFooter.MenuFooterIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                menuFooterID: submitModel.MenuFooterID,
                menuFooter: new MenuFooterIudDTO
                {
                    MenuFooterTitle = submitModel.MenuFooterTitle,
                    MenuFooterTitleEng = submitModel.MenuFooterTitleEng,
                    PageID = submitModel.PageID,
                    MenuFooterIsExternalPage = submitModel.MenuFooterIsExternalPage,
                    MenuFooterExternalPageUrl = submitModel.MenuFooterExternalPageUrl,
                    MenuFooterIsPublished = submitModel.MenuFooterIsPublished,
                    MenuFooterIsTargetBlank = submitModel.MenuFooterIsTargetBlank
                }
            );
            isError = repositoryMenuFooter.IsError;

            if (!isError && !submitModel.MenuFooterIsExternalPage)
            {
                var repositoryPages = RepositoriesFactory.CreatePagesRepository();
                await repositoryPages.PagesIUD(
                    databaseAction: Enums.DatabaseActions.UPDATE,
                    pageID: submitModel.PageID,
                    page: new PageIudDTO
                    {
                        PageID = submitModel.PageID,
                        PageTitle = submitModel.PageTitle,
                        PageTitleEng = submitModel.PageTitleEng,
                        PageSlug = submitModel.PageSlug,
                        PageIsPublished = submitModel.PageIsPublished
                    }
                );
                isError = repositoryPages.IsError;
            }

            if (!isError)
            {
                viewModel.IsSuccess = true;
                var dbItem = await repositoryMenuFooter.MenuFooterGetSingleByID(menuFooterID: submitModel.MenuFooterID);
                if (dbItem != null)
                {
                    var partialViewModel = getMenuListItemFromMenuFooterDto(dbItem);
                    viewModel.IsSuccess = true;
                    viewModel.Data = await WebUtilities.RenderViewAsync(
                        controller: Controller,
                        contentRootPath: Utilities.ContentRootPath,
                        viewName: ViewNames.Admin.MenuFooter.MenuFooterTreeNodePartial,
                        model: partialViewModel
                    );
                }
            }
            return viewModel;
        }

        public async Task<AjaxResponse> Sort(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateMenuFooterRepository();
            await repository.MenuFooterSort(sortIndexes: submitModel.SortIndexes);
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }

        public async Task<AjaxResponse> Delete(SubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateMenuFooterRepository();
            await repository.MenuFooterIUD(
                databaseAction: Enums.DatabaseActions.DELETE,
                menuFooterID: submitModel.MenuFooterID,
                menuFooter: null
            );
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }

        public async Task<AjaxResponse> Get(int? menuFooterID)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateMenuFooterRepository();
            var dbItem = await repository.MenuFooterGetSingleByID(menuFooterID: menuFooterID);
            if (dbItem != null)
            {
                viewModel.IsSuccess = true;
                viewModel.Data = new
                {
                    MenuFooterID = dbItem.MenuFooterID,                    
                    MenuFooterTitle = dbItem.MenuFooterTitle,
                    MenuFooterTitleEng = dbItem.MenuFooterTitleEng,
                    MenuFooterIsExternalPage = dbItem.MenuFooterIsExternalPage,
                    MenuFooterExternalPageUrl = dbItem.MenuFooterExternalPageUrl,
                    MenuFooterIsPublished = dbItem.MenuFooterIsPublished,
                    MenuFooterIsTargetBlank = dbItem.MenuFooterIsTargetBlank,

                    PageID = dbItem.PageID,
                    PageSlug = dbItem.PageSlug,
                    PageTitle = dbItem.PageTitle,
                    PageTitleEng = dbItem.PageTitleEng,
                    PageIsPublished = dbItem.PageIsPublished
                };
            }

            return viewModel;
        }

        public async Task<ValidationResult63> ValidateSubmitModel(SubmitModel submitModel)
        {
            var validationResult = new ValidationResult63();

            if (submitModel.MenuFooterIsExternalPage)
            {
                validationResult.AddError(
                       Validation63.ValidateRequired(errorKey: ".js-modal-MenuFooterExternalPageUrl-input", valueToValidate: submitModel.MenuFooterExternalPageUrl)
                   );
                validationResult.AddError(
                    Validation63.ValidateRequired(errorKey: ".js-modal-MenuFooterTitle-input", valueToValidate: submitModel.MenuFooterTitle)
                );
            }
            else
            {
                validationResult.AddError(
                    Validation63.ValidateRequired(errorKey: ".js-modal-PageID-input", valueToValidate: submitModel.PageID)
                );
                validationResult.AddError(
                    Validation63.ValidateRequired(errorKey: ".js-modal-PageTitle-input", valueToValidate: submitModel.PageTitle)
                );
                validationResult.AddError(
                    Validation63.ValidateRequired(errorKey: ".js-modal-PageSlug-input", valueToValidate: submitModel.PageSlug)
                );
                validationResult.AddError(
                    await Validation63.ValidateAsync(
                        errorAction: async () =>
                        {
                            var repository = RepositoriesFactory.CreatePagesRepository();
                            var isUniq = await repository.PagesIsSlugUniq(pageSlug: submitModel.PageSlug, pageID: submitModel.PageID);
                            var isError = !isUniq;
                            return isError;
                        },
                        errorKey: ".js-modal-PageSlug-input",
                        errorMessage: Resources.ValidationPagesSlugNotUniq
                    )
                );
            }

            return validationResult;
        }

        ViewModel.MenuListItem getMenuListItemFromMenuFooterDto(MenuFooterDTO menuFooterDto)
        {
            var menuListItem = new ViewModel.MenuListItem();
            menuListItem.MenuFooterID = menuFooterDto.MenuFooterID;
            if (menuFooterDto.MenuFooterIsExternalPage)
            {
                var titles = new List<string>(3) { menuFooterDto.MenuFooterTitle, menuFooterDto.MenuFooterTitleEng };
                titles.RemoveAll(title => string.IsNullOrWhiteSpace(title));
                var publishedIcon = menuFooterDto.MenuFooterIsPublished ? "✔️" : "🚫";
                menuListItem.MenuFooterTitle = $"{string.Join(" | ", titles)} {publishedIcon}";
            }
            else
            {                
                var titles = new List<string>(3) { menuFooterDto.PageTitle, menuFooterDto.PageTitleEng };
                titles.RemoveAll(title => string.IsNullOrWhiteSpace(title));
                var publishedIcon = menuFooterDto.PageIsPublished ? "✔️" : "🚫";
                menuListItem.MenuFooterTitle = $"{string.Join(" | ", titles)} {publishedIcon}";
            }
            menuListItem.ShowUpdateButton = _showUpdateButton;
            menuListItem.ShowDeleteButton = _showDeleteButton;
            return menuListItem;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public List<KeyValueTuple<int?, string>> Pages { get; set; }
            public List<MenuListItem> MenuItems { get; set; }
            public bool HasMenuItems => MenuItems?.Any() == true;
            public bool ShowAddNewButton { get; set; }
            public string UrlAdd { get; set; }
            public string UrlUpdate { get; set; }
            public string UrlDelete { get; set; }
            public string UrlSort { get; set; }
            public string UrlGet { get; set; }
            public string UrlGetPage { get; set; }
            public string UrlGetPages { get; set; }

            public readonly string SelectBoxValueExpr = nameof(KeyValueTuple<object, object>.Key);
            public readonly string SelectBoxDisplayExpr = nameof(KeyValueTuple<object, object>.Value);
            public readonly string TextAdd = Resources.TextAdd;
            public readonly string TextUpdate = Resources.TextUpdate;
            public readonly string TextPageInternal = Resources.TextPageInternal;
            public readonly string TextPageExternalUrl = Resources.TextPageExternalUrl;
            public readonly string TextTitle = Resources.TextTitle;
            public readonly string TextTitleEng = Resources.TextTitleEng;            
            public readonly string TextSlug = Resources.TextSlug;
            public readonly string TextGenerateFromTitle = Resources.TextGenerateFromTitle;            
            public readonly string TextPublished = Resources.TextPublished;
            public readonly string TextOpenInNewTab = Resources.TextOpenInNewTab;
            public readonly string TextPageIsExternal = Resources.TextPageIsExternal;
            public readonly string TextConfirmDelete = Resources.TextConfirmDelete;
            public readonly string TextConfirmDeleteRecursiveWithTypeDelete = Resources.TextConfirmDeleteRecursiveWithTypeDelete;
            #endregion

            #region Nested Classes
            public record MenuListItem
            {
                #region Properties
                public int? MenuFooterID { get; set; }
                public string MenuFooterTitle { get; set; }                
                public bool ShowUpdateButton { get; set; }
                public bool ShowDeleteButton { get; set; }
                public List<MenuListItem> Children { get; set; }
                public bool HasChildren => Children?.Any() == true;
                #endregion
            }
            #endregion
        }

        public record SubmitModel
        {
            #region Properties
            public int? MenuFooterID { get; init; }
            public string MenuFooterTitle { get; init; }
            public string MenuFooterTitleEng { get; init; }
            public bool MenuFooterIsExternalPage { get; init; }
            public string MenuFooterExternalPageUrl { get; init; }
            public bool MenuFooterIsPublished { get; init; }
            public bool MenuFooterIsTargetBlank { get; set; }

            public int? PageID { get; init; }
            public string PageTitle { get; init; }
            public string PageTitleEng { get; init; }            
            public string PageSlug { get; init; }
            public bool PageIsPublished { get; init; }
            #endregion
        }
        #endregion
    }
}
