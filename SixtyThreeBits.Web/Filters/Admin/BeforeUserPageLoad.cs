using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Admin;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Admin;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Filters.Admin
{
    public class BeforeUserPageLoad : IAsyncActionFilter
    {
        #region Properties
        UserModelBase _model;
        readonly UserLayoutViewModel _viewModel = new();
        #endregion

        #region Methods
        public BeforeUserPageLoad()
        {
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _model = WebUtilities.GetModelFromController<UserModelBase>(filterContext.Controller);
            var c = filterContext.Controller as Controller;
            var userID = filterContext.RouteData.Values[WebConstants.RouteValues.UserID]?.ToString().ToInt();

            var repository = _model.RepositoriesFactory.GetUsersRepository();
            _model.dbItem = await repository.UsersGetSingleByID(userID);

            if (_model.dbItem == null)
            {
                filterContext.Result = _model.GetNotFoundAdminViewResult();
            }
            else
            {
                if (!_model.IsAjaxRequest)
                {
                    InitStartup();
                    InitPageTitle();
                    ReinitBreadCrumbs();
                    InitTabs();
                    WebUtilities.SetLayoutViewModel(viewData: c.ViewData, viewModel: _viewModel, key: WebConstants.ViewData.UserLayoutViewModel);
                }
                await next();
            }
        }

        void InitStartup()
        {
            _viewModel.UserFullname = _model.dbItem.UserFullname;
            _viewModel.UserIsActive = _model.dbItem.UserIsActive;
            _viewModel.UserDateCreated = _model.Utilities.FormatDateTime(_model.dbItem.UserDateCreated);
            _viewModel.RoleName = _model.dbItem.RoleName;
        }

        void InitPageTitle()
        {
            _model.PageTitle.Set(_model.dbItem.UserFullname);
        }

        void ReinitBreadCrumbs()
        {
            _model.Breadcrumbs.DeleteLastItem();
            _model.Breadcrumbs.RenameLastItem(_model.dbItem.UserFullname);
        }

        void InitTabs()
        {
            var tabsParentID = _model.User.Permissions.FindLast(Item => Item.PermissionCodeName == ControllerActionRouteNames.Admin.Users.User.Root)?.PermissionID;

            if (tabsParentID != null)
            {
                var tabs = _model.User.Permissions
                .Where(item => item.PermissionIsMenuItem && item.PermissionParentID == tabsParentID)
                .OrderBy(item => item.PermissionSortIndex)
                .Select(item => new ProjectMenuViewItem
                {
                    Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, item.PermissionMenuTitleOrCaption, item.PermissionMenuTitleOrCaptionEng),
                    NavigateUrl = _model.Url.RouteUrl(item.PermissionCodeName, new { userID = _model.dbItem.UserID }),
                    IsSelected = Regex.IsMatch(_model.UrlCurrentPageWithDomain, item.PermissionPagePath)
                }).ToList();

                if (tabs?.Count > 0)
                {
                    _viewModel.Tabs = [];
                    var selectedItem = tabs.FirstOrDefault(Item => Item.IsSelected);
                    if (selectedItem != null)
                    {
                        selectedItem.NavigateUrl = null;
                    }
                    _viewModel.Tabs.AddRange(tabs);
                }
            }
        }
        #endregion
    }
}