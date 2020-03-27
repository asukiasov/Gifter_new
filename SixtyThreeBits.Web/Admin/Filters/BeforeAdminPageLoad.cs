using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Linq;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforeAdminPageLoad : ActionFilterAttribute
    {
        WebProjectModelBase Model;
        AdminLayoutViewModel ViewModel;

        public override void OnActionExecuted(ActionExecutedContext FilterContext)
        {

        }

        public override void OnActionExecuting(ActionExecutingContext FilterContext)
        {
            ViewModel = new AdminLayoutViewModel();
            Model = WebsiteUtilities.GetWebProjectModelBaseFromController(FilterContext.Controller);
            var C = FilterContext.Controller as Controller;

            var IsAuthorized = AdminAuthorize();            
            if (IsAuthorized)
            {
                InitStartUp();
                InitClientPlugins();
                InitMenu();
                InitBreadCrumbs();
                InitPageTitle();
                InitSuccessErrorMessage();
                InitSidebar();

                WebsiteUtilities.SetLayoutViewModel(ViewData: C.ViewData, ViewModel: ViewModel, Key: Constants.ViewData.LayoutViewModel);
            }
            else
            {
                var UrlLogin = Model.Url.RouteUrl(ControllerActionRouteNames.Admin.Auth.Login);
                FilterContext.Result = new RedirectResult(UrlLogin);
            }
        }

        bool AdminAuthorize() 
        {
            var IsAuthorized = false;
            Model.User = Model.SessionAssistance.Get<User>(Constants.Session.User);

            if (Model.AppSettings.IsDevelopment)
            {
                if (Model.User == null)
                {
                    var UserID = Model.CookieAssistance.Get<int?>(Constants.Cookies.User);
                    if (UserID != null)
                    {
                        Model.User = Model.DataAccessFactory.Users.GetSingleUserByID(UserID).Result;
                        Model.SessionAssistance.Set(Constants.Session.User, Model.User);
                    }
                }
            }

            if (Model.User != null)
            {
                ViewModel.UserFullname = Model.User.UserFullname;
                IsAuthorized = true;
            }

            return IsAuthorized;
        }

        void InitStartUp()
        {
            Model.Language = Constants.Languages.ENGLISH;            
        }

        void InitClientPlugins()
        {
            Model.PluginClient
            .EnableGoogleFonts(true)
            .EnableFontAwesome(true)
            .Enable63BitsFonts(true)
            .EnableBootstrap(true)
            .EnableAngle(true)
            .EnableJQuery(true)
            .EnableJQueryConfirm(true)
            .EnablePreloader(true)
            .Enable63BitsComponents(true);

            ViewModel.PluginClient = Model.PluginClient;
        }

        void InitMenu()
        {
            if (Model.User.Permissions?.Count > 0)
            {                
                ViewModel.Menu = Model.User.Permissions
                .Where(Item => Item.PermissionIsMenuItem && Item.PermissionParentID == null)
                .Select(Item => new ProjectMenuItem
                {
                    Caption = Item.PermissionCaption,
                    NavigateUrl = string.IsNullOrWhiteSpace(Item.PermissionPagePath) ? Item.PermissionCode : Item.PermissionPagePath,
                    Icon = Item.PermissionMenuIcon,
                    IsSelected = Item.PermissionPagePath == Model.UrlCurrentPage,
                    Children = Model.User.Permissions.Where(SubItem => SubItem.PermissionIsMenuItem && SubItem.PermissionParentID == Item.PermissionID).Select(SubItem => new ProjectMenuItem
                    {
                        Caption = SubItem.PermissionCaption,
                        NavigateUrl = SubItem.PermissionPagePath,
                        Icon = SubItem.PermissionMenuIcon,
                        IsSelected = SubItem.PermissionPagePath == Model.UrlCurrentPage
                    }).ToList()
                }).ToList();

                ViewModel.Menu.ForEach(Item =>
                {
                    if(Item.HasChildren)
                    {
                        Item.IsSelected = Item.Children.Any(SubItem => SubItem.IsSelected);
                    }
                });
            }

            ViewModel.UrlRelogin = Model.Url.RouteUrl(ControllerActionRouteNames.Admin.Auth.Relogin);
            ViewModel.UrlLogout = Model.Url.RouteUrl(ControllerActionRouteNames.Admin.Auth.Logout);
        }

        void InitBreadCrumbs()
        {
            Model.Breadcrumbs = Breadcrumbs.GetBreadcrumbsByPageUrl(Model.User.Permissions, Model.UrlCurrentPage);
            ViewModel.Breadcrumbs = Model.Breadcrumbs;
        }

        void InitPageTitle()
        {
            var P = Model.User.GetPermission(Model.UrlCurrentPage);
            if (P != null)
            {
                Model.SetPageTitle(P.PermissionCaption);
            }

            ViewModel.PageTitle = Model.PageTitle;
        }

        void InitSidebar()
        {
            Model.IsSidebarCollapsed = new ValueReference<bool>
            {
                Value = Model.CookieAssistance.Get<bool>(Key: Constants.Cookies.IsAdminSideBarCollapsed)
            };
            ViewModel.IsSidebarCollapsed = Model.IsSidebarCollapsed;
        }

        void InitSuccessErrorMessage()
        {
            Model.InitSuccessErrorPartialViewModel();
            ViewModel.SuccessErrorPartialViewModel = Model.SuccessErrorPartialViewModel;
        }
    }
}