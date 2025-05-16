using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Admin;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Base;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Filters.Admin
{
    public class AdminFilterAttribute : IAsyncActionFilter
    {
        #region Properties
        ModelBase _model;
        AdminLayoutViewModel _viewModel;
        #endregion

        #region Methods
        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _viewModel = new AdminLayoutViewModel();
            _model = WebUtilities.GetModelFromController<ModelBase>(filterContext.Controller);
            var c = filterContext.Controller as Controller;

            var isAuhenticated = isUserAuhenticated();
            if (isAuhenticated)
            {
                var hasPermission = hasUserPermission();
                if (hasPermission)
                {
                    if (!_model.IsAjaxRequest)
                    {
                        initStartUp();
                        initLanguage();
                        initClientPlugins();
                        initMenu();
                        initBreadCrumbs();
                        initTabs();
                        initPageTitle();
                        initSuccessErrorToast();
                        initSidebar();
                        WebUtilities.SetLayoutViewModel(viewData: c.ViewData, viewModel: _viewModel, key: WebConstants.ViewData.LayoutViewModel);
                    }
                    await next();
                }
                else
                {
                    filterContext.Result = _model.GetNotFoundAdminViewResult();
                }
            }
            else
            {
                var urlLogin = _model.Url.RouteUrl(ControllerActionRouteNames.Admin.AuthController.Login);
                filterContext.Result = new RedirectResult(urlLogin);
            }
        }

        bool isUserAuhenticated()
        {
            return _model.User != null;
        }

        bool hasUserPermission()
        {
            var hasPermission = _model.User.HasPermission(_model.UrlCurrentPageWithoutDomain);
            return hasPermission;
        }

        void initStartUp()
        {
            _viewModel.ProjectName = _model.SystemProperties.ProjectName;
            _viewModel.UserFullname = _model.User.UserFullname;
            _viewModel.UserEmail = _model.User.UserEmail;
        }

        void initClientPlugins()
        {
            _model.PluginsClient
            //.EnableGoogleFonts(true)
            .Enable63BitsFonts(true)
            .EnableFontAwesome(true)
            .EnableAdminTheme(true)
            .EnableJQuery(true)
            .EnableJQueryConfirm(true)
            .EnablePreloader(true)
            .Enable63BitsComponents(true)
            .EnableMetisMenu(true)
            .Enable63BitsAnalogClock(true)
            .EnableUtils(true);

            _viewModel.PluginsClient = _model.PluginsClient;
        }

        void initMenu()
        {
            if (_model.User.Permissions?.Count > 0)
            {
                _viewModel.Menu = _model.User.Permissions
                .Where(item => item.PermissionIsMenuItem && item.PermissionParentID == null)
                .Select(item => new ProjectMenuViewItem
                {
                    Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, item.PermissionMenuTitleOrCaption, item.PermissionMenuTitleOrCaptionEng),
                    NavigateUrl = string.IsNullOrWhiteSpace(item.PermissionPagePath) ? item.PermissionCode : item.PermissionPagePath,
                    Icon = item.PermissionMenuIcon,
                    IsSelected = item.PermissionPagePath == _model.UrlCurrentPageWithoutDomain,
                    Children = _model.User.Permissions.Where(subItem => subItem.PermissionIsMenuItem && subItem.PermissionParentID == item.PermissionID).Select(subItem => new ProjectMenuViewItem
                    {
                        Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, subItem.PermissionMenuTitleOrCaption, subItem.PermissionMenuTitleOrCaptionEng),
                        NavigateUrl = subItem.PermissionPagePath,
                        Icon = subItem.PermissionMenuIcon,
                        IsSelected = subItem.PermissionPagePath == _model.UrlCurrentPageWithoutDomain
                    }).ToList()
                }).ToList();

                _viewModel.Menu.ForEach(item =>
                {
                    if (item.HasChildren)
                    {
                        item.IsSelected = item.Children.Any(subItem => subItem.IsSelected);
                    }
                });
            }

            _viewModel.UrlRelogin = _model.Url.RouteUrl(ControllerActionRouteNames.Admin.AuthController.Relogin);
            _viewModel.UrlLogout = _model.Url.RouteUrl(ControllerActionRouteNames.Admin.AuthController.Logout);
        }

        void initBreadCrumbs()
        {
            var pageHierarchy = _model.User.Permissions?.Select(item => new Breadcrumbs.HierarchyItem<int?>
            {
                ID = item.PermissionID,
                ParentID = item.PermissionParentID,
                PageHttpPath = item.PermissionPagePath,
                PageTitle = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, item.PermissionMenuTitleOrCaption, item.PermissionMenuTitleOrCaptionEng),
            }).ToList();

            _viewModel.Breadcrumbs = _model.Breadcrumbs = new Breadcrumbs();
            _viewModel.Breadcrumbs.InitBreadcrumbsByPageUrl(
                pageHierarchy: pageHierarchy,
                urlCurrentPage: _model.UrlCurrentPageWithDomain
            );
            _viewModel.ShowBreadCrumbs = _viewModel.Breadcrumbs.ItemsCount > 2;
        }

        void initTabs()
        {
            _viewModel.Tabs = _model.Tabs;
        }

        void initPageTitle()
        {
            _model.PageTitle = _viewModel.PageTitle = new PageTitle(_model.SystemProperties.ProjectName);
            var p = _model.User.GetPermission(_model.UrlCurrentPageWithoutDomain);
            if (p != null)
            {
                _model.PageTitle.Set(_model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, p.PermissionMenuTitleOrCaption, p.PermissionMenuTitleOrCaptionEng));
            }
        }

        void initSidebar()
        {
            _viewModel.IsSidebarCollapsed = _model.IsSidebarCollapsed = new ValueWrapper<bool>();
            _model.IsSidebarCollapsed.Value = _model.CookieAssistance.Get(key: WebConstants.Cookies.IsAdminSideBarCollapsed).ToBooleanValue();
        }

        void initSuccessErrorToast()
        {
            _model.InitSuccessErrorToastNotificationPartialViewModel();
            _viewModel.SuccessErrorPartialViewModel = _model.SuccessErrorPartialViewModel;
        }

        void initLanguage()
        {
            var language = _model.Utilities.GetSupportedLanguageOrDefault(_model.LanguageCultureCode);

            _viewModel.LanguageActive = new AdminLayoutViewModel.Language { LanguageCultureCode = language.LanguageCultureCode, LanguageName = language.LanguageName };
            _viewModel.Languages = _model.Utilities.SupportedLanguages.Select(item => new AdminLayoutViewModel.Language
            {
                LanguageCultureCode = item.LanguageCultureCode,
                LanguageName = item.LanguageName,
                IsActive = item.LanguageCultureCode == language.LanguageCultureCode,
                UrlChangeLanguage = _model.Url.RouteUrl(ControllerActionRouteNames.Admin.ChangeLanguageController.ChangeLanguage, new { Culture = item.LanguageCultureCode })
            }).ToList();
        }
        #endregion
    }
}