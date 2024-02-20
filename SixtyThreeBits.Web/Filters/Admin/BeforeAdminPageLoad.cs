using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using SixtyThreeBits.Web.Models.Admin;
using SixtyThreeBits.Web.Models.Shared;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Filters.Admin
{
    public class BeforeAdminPageLoad : IAsyncActionFilter
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
                    initStartUp();
                    initLanguage();
                    initClientPlugins();
                    initMenu();
                    initBreadCrumbs();
                    initTabs();
                    initPageTitle();
                    initSuccessErrorMessage();
                    initSidebar();
                    WebUtilities.SetLayoutViewModel(viewData: c.ViewData, viewModel: _viewModel, key: WebConstants.ViewData.LayoutViewModel);
                    await next();
                }
                else
                {
                    filterContext.Result = _model.GetNotFoundAdminViewResult();
                }
            }
            else
            {
                var urlLogin = _model.Url.RouteUrl(ControllerActionRouteNames.Admin.Auth.Login);
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
            .EnableUtils(true);

            _viewModel.PluginsClient = _model.PluginsClient;
        }

        void initMenu()
        {
            if (_model.User.Permissions?.Count > 0)
            {
                _viewModel.Menu = _model.User.Permissions
                .Where(item => item.PermissionIsMenuItem && item.PermissionParentID == null)
                .Select(item => new ProjectMenuItem
                {
                    Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, item.HasPermissionMenuTitle ? item.PermissionMenuTitle : item.PermissionCaption, item.HasPermissionMenuTitleEng ? item.PermissionMenuTitleEng : item.PermissionCaptionEng),
                    NavigateUrl = string.IsNullOrWhiteSpace(item.PermissionPagePath) ? item.PermissionCode : item.PermissionPagePath,
                    Icon = item.PermissionMenuIcon,
                    IsSelected = item.PermissionPagePath == _model.UrlCurrentPageWithoutDomain,
                    Children = _model.User.Permissions.Where(subItem => subItem.PermissionIsMenuItem && subItem.PermissionParentID == item.PermissionID).Select(SubItem => new ProjectMenuItem
                    {
                        Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, SubItem.HasPermissionMenuTitle ? SubItem.PermissionMenuTitle : SubItem.PermissionCaption, SubItem.HasPermissionMenuTitleEng ? SubItem.PermissionMenuTitleEng : SubItem.PermissionCaptionEng),
                        NavigateUrl = SubItem.PermissionPagePath,
                        Icon = SubItem.PermissionMenuIcon,
                        IsSelected = SubItem.PermissionPagePath == _model.UrlCurrentPageWithoutDomain
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

            _viewModel.UrlRelogin = _model.Url.RouteUrl(ControllerActionRouteNames.Admin.Auth.Relogin);
            _viewModel.UrlLogout = _model.Url.RouteUrl(ControllerActionRouteNames.Admin.Auth.Logout);
        }

        void initBreadCrumbs()
        {
            var pageHierarchy = _model.User.Permissions?.Select(item => new Breadcrumbs.HierarchyItem<int?>
            {
                ID = item.PermissionID,
                ParentID = item.PermissionParentID,
                PageHttpPath = item.PermissionPagePath,
                PageTitle = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, item.PermissionCaption, item.PermissionCaptionEng),
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
                _model.PageTitle.Set(_model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, p.PermissionCaption, p.PermissionCaptionEng));
            }
        }

        void initSidebar()
        {
            _viewModel.IsSidebarCollapsed = _model.IsSidebarCollapsed = new ValueWrapper<bool>();
            _model.IsSidebarCollapsed.Value = _model.CookieAssistance.Get<bool>(Key: WebConstants.Cookies.IsAdminSideBarCollapsed);
        }

        void initSuccessErrorMessage()
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
                UrlChangeLanguage = _model.Url.RouteUrl(ControllerActionRouteNames.Admin.ChangeLanguage.Page, new { Culture = item.LanguageCultureCode })
            }).ToList();
        }
        #endregion
    }
}