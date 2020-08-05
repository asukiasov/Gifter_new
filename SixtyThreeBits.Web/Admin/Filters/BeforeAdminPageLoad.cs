using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforeAdminPageLoad : IAsyncActionFilter
    {
        WebProjectModelBase Model;
        AdminLayoutViewModel ViewModel;

        public async Task OnActionExecutionAsync(ActionExecutingContext FilterContext, ActionExecutionDelegate next)
        {
            ViewModel = new AdminLayoutViewModel();
            Model = LocalUtilities.GetModelFromController<WebProjectModelBase>(FilterContext.Controller);
            var C = FilterContext.Controller as Controller;

            var IsAuthorized = await AdminAuthorize();            
            if (IsAuthorized)
            {
                InitStartUp();
                InitClientPlugins();
                InitMenu();
                InitBreadCrumbs();
                InitTabs();
                InitPageTitle();
                InitSuccessErrorMessage();
                InitSidebar();                
                LocalUtilities.SetLayoutViewModel(ViewData: C.ViewData, ViewModel: ViewModel, Key: Constants.ViewData.LayoutViewModel);
                await next();
            }
            else
            {
                var UrlLogin = Model.Url.RouteUrl(ControllerActionRouteNames.Admin.Auth.Login);
                FilterContext.Result = new RedirectResult(UrlLogin);
            }            
        }

        async Task<bool> AdminAuthorize() 
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
                        Model.User = await Model.DataAccessFactory.Users.GetSingleUserByID(UserID);
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
            Model.Culture = Constants.Languages.ENGLISH;
            
        }

        void InitClientPlugins()
        {
            Model.PluginsClient
            .EnableGoogleFonts(true)
            .EnableFontAwesome(true)
            .Enable63BitsFonts(true)
            .EnableBootstrap(true)
            .EnableAngle(true)
            .EnableJQuery(true)
            .EnableJQueryConfirm(true)
            .EnablePreloader(true)
            .Enable63BitsComponents(true)
            .EnableUtils(true);

            ViewModel.PluginsClient = Model.PluginsClient;
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
            var PageHierarchy = Model.User.Permissions?.Select(Item => new Breadcrumbs.HierarchyItem<int?>
            {
                ID = Item.PermissionID,
                ParentID = Item.PermissionParentID,
                PageHttpPath = Item.PermissionPagePath,
                PageTitle = Item.PermissionCaption
            }).ToList();

            ViewModel.Breadcrumbs = Model.Breadcrumbs = Breadcrumbs.GetBreadcrumbsByPageUrl(
                PageHierarchy: PageHierarchy,
                UrlCurrentPage: Model.UrlCurrentPage
            );
        }        

        void InitTabs()
        {
            ViewModel.Tabs = Model.Tabs;
        }

        void InitPageTitle()
        {
            Model.PageTitle = ViewModel.PageTitle = new PageTitle();
            var P = Model.User.GetPermission(Model.UrlCurrentPage);
            if (P != null)
            {
                Model.PageTitle.Set(P.PermissionCaption);
            }            
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