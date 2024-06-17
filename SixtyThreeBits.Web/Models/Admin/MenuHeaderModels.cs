using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class MenuHeaderModel : ModelBase
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
            var repositoryMenuHeader = RepositoriesFactory.CreateMenuHeaderRepository();
            var repositoryPages = RepositoriesFactory.CreatePagesRepository();

            _showAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuHeaderController.Add);
            _showUpdateButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuHeaderController.Update);
            _showDeleteButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuHeaderController.Delete);

            viewModel.ShowAddNewButton = _showAddNewButton;
            viewModel.UrlAdd = Url.RouteUrl(ControllerActionRouteNames.Admin.MenuHeaderController.Add);
            viewModel.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.MenuHeaderController.Update);
            viewModel.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.MenuHeaderController.Delete);
            viewModel.UrlSort = Url.RouteUrl(ControllerActionRouteNames.Admin.MenuHeaderController.Sort);
            viewModel.UrlGet = Url.RouteUrl(ControllerActionRouteNames.Admin.MenuHeaderController.Get, new { menuHeaderID = 0 }).TrimEnd('0').TrimEnd('/');
            viewModel.UrlGetPage = Url.RouteUrl(ControllerActionRouteNames.Admin.PageDataController.Get, new { pageID = 0 }).TrimEnd('0').TrimEnd('/');
            viewModel.UrlGetPages = Url.RouteUrl(ControllerActionRouteNames.Admin.PagesController.Get);

            viewModel.Pages = (await repositoryPages.PagesList())?
            .OrderBy(item => item.PageTitle)
            .Select(item => new KeyValueTuple<int?, string>
            {
                Key = item.PageID,
                Value = $"{string.Join(" | ", item.PageTitle, item.PageTitleEng)}"
            })
            .ToList();

            var menuItems = (await repositoryMenuHeader.MenuHeaderList())
            ?.Select(item => getMenuListItemFromMenuHeaderDto(item))
            .ToList();

            if (menuItems?.Any() == true)
            {
                menuItems.ToRecursive(
                    IDPropertyName: nameof(ViewModel.MenuListItem.MenuHeaderID), 
                    parentIDPropertyName: nameof(ViewModel.MenuListItem.MenuHeaderParentID),
                    childrenPropertyName: nameof(ViewModel.MenuListItem.Children)
                );
                viewModel.MenuItems = menuItems;
            }

            return viewModel;
        }

        public async Task<AjaxResponse> Add(SubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repositoryMenuHeader = RepositoriesFactory.CreateMenuHeaderRepository();
            var isError = false;

            _showAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuHeaderController.Add);
            _showUpdateButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuHeaderController.Update);
            _showDeleteButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuHeaderController.Delete);

            var menuHeaderID = await repositoryMenuHeader.MenuHeaderIUD(
                databaseAction: Enums.DatabaseActions.CREATE,
                menuHeaderID: null,
                menuHeader: new MenuHeaderIudDTO
                {
                    MenuHeaderParentID = submitModel.MenuHeaderParentID,
                    MenuHeaderTitle = submitModel.MenuHeaderTitle,
                    MenuHeaderTitleEng = submitModel.MenuHeaderTitleEng,
                    PageID = submitModel.PageID,
                    MenuHeaderIsExternalPage = submitModel.MenuHeaderIsExternalPage,
                    MenuHeaderExternalPageUrl = submitModel.MenuHeaderExternalPageUrl,
                    MenuHeaderIsPublished = submitModel.MenuHeaderIsPublished,
                    MenuHeaderIsTargetBlank = submitModel.MenuHeaderIsTargetBlank
                }
            );
            isError = repositoryMenuHeader.IsError;

            if (!isError && !submitModel.MenuHeaderIsExternalPage)
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
                var dbItem = await repositoryMenuHeader.MenuHeaderGetSingleByID(menuHeaderID: menuHeaderID);
                if (dbItem != null)
                {
                    var partialViewModel = getMenuListItemFromMenuHeaderDto(dbItem);
                    viewModel.IsSuccess = true;
                    viewModel.Data = await WebUtilities.RenderViewAsync(
                        controller: Controller,
                        contentRootPath: Utilities.ContentRootPath,
                        viewName: ViewNames.Admin.MenuHeader.MenuHeaderTreeNodePartialView,
                        model: partialViewModel
                    );
                }
            }
            return viewModel;
        }

        public async Task<AjaxResponse> Update(SubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repositoryMenuHeader = RepositoriesFactory.CreateMenuHeaderRepository();
            var isError = false;

            _showAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuHeaderController.Add);
            _showUpdateButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuHeaderController.Update);
            _showDeleteButton = User.HasPermission(ControllerActionRouteNames.Admin.MenuHeaderController.Delete);

            await repositoryMenuHeader.MenuHeaderIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                menuHeaderID: submitModel.MenuHeaderID,
                menuHeader: new MenuHeaderIudDTO
                {
                    MenuHeaderParentID = submitModel.MenuHeaderParentID,
                    MenuHeaderTitle = submitModel.MenuHeaderTitle,
                    MenuHeaderTitleEng = submitModel.MenuHeaderTitleEng,
                    PageID = submitModel.PageID,
                    MenuHeaderIsExternalPage = submitModel.MenuHeaderIsExternalPage,
                    MenuHeaderExternalPageUrl = submitModel.MenuHeaderExternalPageUrl,
                    MenuHeaderIsPublished = submitModel.MenuHeaderIsPublished,
                    MenuHeaderIsTargetBlank = submitModel.MenuHeaderIsTargetBlank
                }
            );
            isError = repositoryMenuHeader.IsError;

            if (!isError && !submitModel.MenuHeaderIsExternalPage)
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
                var dbItem = await repositoryMenuHeader.MenuHeaderGetSingleByID(menuHeaderID: submitModel.MenuHeaderID);
                if (dbItem != null)
                {
                    var partialViewModel = getMenuListItemFromMenuHeaderDto(dbItem);
                    viewModel.IsSuccess = true;
                    viewModel.Data = await WebUtilities.RenderViewAsync(
                        controller: Controller,
                        contentRootPath: Utilities.ContentRootPath,
                        viewName: ViewNames.Admin.MenuHeader.MenuHeaderTreeNodePartialView,
                        model: partialViewModel
                    );
                }
            }
            return viewModel;
        }

        public async Task<AjaxResponse> Sort(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateMenuHeaderRepository();
            await repository.MenuHeaderSort(sortIndexes: submitModel.SortIndexes);
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }

        public async Task<AjaxResponse> Delete(SubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateMenuHeaderRepository();
            await repository.MenuHeaderIUD(
                databaseAction: Enums.DatabaseActions.DELETE,
                menuHeaderID: submitModel.MenuHeaderID,
                menuHeader: null
            );
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }

        public async Task<AjaxResponse> Get(int? menuHeaderID)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateMenuHeaderRepository();
            var dbItem = await repository.MenuHeaderGetSingleByID(menuHeaderID: menuHeaderID);
            if (dbItem != null)
            {
                viewModel.IsSuccess = true;
                viewModel.Data = new
                {
                    MenuHeaderID = dbItem.MenuHeaderID,
                    MenuHeaderParentID = dbItem.MenuHeaderParentID,
                    MenuHeaderTitle = dbItem.MenuHeaderTitle,
                    MenuHeaderTitleEng = dbItem.MenuHeaderTitleEng,
                    MenuHeaderIsExternalPage = dbItem.MenuHeaderIsExternalPage,
                    MenuHeaderExternalPageUrl = dbItem.MenuHeaderExternalPageUrl,
                    MenuHeaderIsPublished = dbItem.MenuHeaderIsPublished,
                    MenuHeaderIsTargetBlank = dbItem.MenuHeaderIsTargetBlank,

                    PageID = dbItem.PageID,
                    PageSlug = dbItem.PageSlug,
                    PageTitle = dbItem.PageTitle,
                    PageTitleEng = dbItem.PageTitleEng,
                    PageIsPublished = dbItem.PageIsPublished
                };
            }

            return viewModel;
        }

        public async Task<Errors> ValidateSubmitModel(SubmitModel submitModel)
        {
            var errors = new Errors();

            if (submitModel.MenuHeaderIsExternalPage)
            {                
                errors.AddError(
                       Validation.ValidateRequired(errorKey: ".js-modal-MenuHeaderExternalPageUrl-input", valueToValidate: submitModel.MenuHeaderExternalPageUrl)
                   );
                errors.AddError(
                    Validation.ValidateRequired(errorKey: ".js-modal-MenuHeaderTitle-input", valueToValidate: submitModel.MenuHeaderTitle)
                );
            }
            else
            {                
                errors.AddError(
                    Validation.ValidateRequired(errorKey: ".js-modal-PageID-input", valueToValidate: submitModel.PageID)
                );
                errors.AddError(
                    Validation.ValidateRequired(errorKey: ".js-modal-PageTitle-input", valueToValidate: submitModel.PageTitle)
                );
                errors.AddError(
                    Validation.ValidateRequired(errorKey: ".js-modal-PageSlug-input", valueToValidate: submitModel.PageSlug)
                );
                errors.AddError(
                    await Validation.ValidateAsync(
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

            return errors;
        }

        ViewModel.MenuListItem getMenuListItemFromMenuHeaderDto(MenuHeaderDTO menuHeaderDto) 
        {
            var menuListItem = new ViewModel.MenuListItem();
            menuListItem.MenuHeaderID = menuHeaderDto.MenuHeaderID;
            menuListItem.MenuHeaderParentID = menuHeaderDto.MenuHeaderParentID;
            if (menuHeaderDto.MenuHeaderIsExternalPage)
            {
                var titles = new List<string>(2) { menuHeaderDto.MenuHeaderTitle, menuHeaderDto.MenuHeaderTitleEng };
                titles.RemoveAll(title => string.IsNullOrWhiteSpace(title));
                var publishedIcon = menuHeaderDto.MenuHeaderIsPublished ? "✔️" : "🚫";
                menuListItem.MenuHeaderTitle = $"{string.Join(" | ", titles)} {publishedIcon}";
            }
            else
            {                
                var titles = new List<string>(2) { menuHeaderDto.PageTitle, menuHeaderDto.PageTitleEng };
                titles.RemoveAll(title => string.IsNullOrWhiteSpace(title));
                var publishedIcon = menuHeaderDto.PageIsPublished ? "✔️" : "🚫";
                menuListItem.MenuHeaderTitle = $"{string.Join(" | ", titles)} {publishedIcon}";
            }
            menuListItem.ShowAddNewButton = _showAddNewButton;
            menuListItem.ShowUpdateButton = _showUpdateButton;
            menuListItem.ShowDeleteButton = _showDeleteButton;
            return menuListItem;
        }        
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public List<KeyValueTuple<int?,string>> Pages { get; set; }
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
            public readonly string SelectBoxDisplayExpr = nameof(KeyValueTuple<object,object>.Value);
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
                public int? MenuHeaderID { get; set; }
                public int? MenuHeaderParentID { get; set; }
                public string MenuHeaderTitle { get; set; }
                public bool ShowAddNewButton { get; set; }
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
            public int? MenuHeaderID { get; init; }
            public int? MenuHeaderParentID { get; init; }
            public string MenuHeaderTitle { get; init; }
            public string MenuHeaderTitleEng { get; init; }            
            public bool MenuHeaderIsExternalPage { get; init; }
            public string MenuHeaderExternalPageUrl { get; init; }
            public bool MenuHeaderIsPublished { get; init; }
            public bool MenuHeaderIsTargetBlank { get; set; }

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