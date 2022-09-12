using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Filters
{
    public class BeforeWebsitePageLoad : IAsyncActionFilter
    {
        WebProjectModelBase Model;
        WebsiteLayoutViewModel ViewModel;

        public async Task OnActionExecutionAsync(ActionExecutingContext FilterContext, ActionExecutionDelegate next)
        {
            ViewModel = new WebsiteLayoutViewModel();
            Model = LocalUtilities.GetModelFromController<WebProjectModelBase>(FilterContext.Controller);
            var C = FilterContext.Controller as Controller;

            InitStartUp(FilterContext);
            await InitSystemProperties();
            InitClientPlugins();
            await InitMenu();
            InitLanguageSwitch();

            LocalUtilities.SetLayoutViewModel(ViewData: C.ViewData, ViewModel: ViewModel, Key: Constants.ViewData.LayoutViewModel);
            await next();
        }

        void InitStartUp(ActionExecutingContext FilterContext)
        {
            Model.Culture = FilterContext.RouteData.Values["Culture"]?.ToString();
            ViewModel.ScriptsHeader = Model.SystemProperties.ScriptsHeader;
            ViewModel.ScriptsBodyStart = Model.SystemProperties.ScriptsBodyStart;
            ViewModel.ScriptsBodyEnd = Model.SystemProperties.ScriptsBodyEnd;
        }

        void InitClientPlugins()
        {
            Model.PluginsClient
            .EnableGoogleFonts(true)
            .EnableFontAwesome(true)
            .Enable63BitsFonts(true)
            .EnableBootstrap(true)
            .EnableJQuery(true)
            .EnablePreloader(true);

            ViewModel.PluginsClient = Model.PluginsClient;
        }

        async Task InitMenu()
        {
            Model.PageTitle = ViewModel.PageTitle = new PageTitle();

            var Pages = await Model.DataAccessFactory.Pages.ListPages(PageIsPublished: true, PageIsMenuItem: true);
            if (Pages != null)
            {
                ViewModel.Menu = new List<ProjectMenuItem>();
                var Parents = Pages.Where(Item => Item.PageParentID == null);
                foreach (var P1 in Parents)
                {
                    var ParentItem = new ProjectMenuItem();
                    ParentItem.Caption = Model.Utilities.GetValuesByLanguage(Model.Culture, P1.PageTitle, P1.PageTitleEng, P1.PageTitleRus);
                    if (P1.PageIsExternalUrl)
                    {
                        ParentItem.NavigateUrl = P1.PageExternalUrl;
                        if (!string.IsNullOrWhiteSpace(P1.PageExternalUrl))
                        {
                            ParentItem.IsTargetBlank = !P1.PageExternalUrl.Contains(Model.WebsiteDomain) && !P1.PageExternalUrl.StartsWith("/");
                        }
                    }
                    else
                    {
                        ParentItem.NavigateUrl = Model.GetRouteByName(ControllerActionRouteNames.Website.Pages.Page, new { Culture = Model.Culture , PageSlugHierarchy = P1.PageSlugHierarchy }); ;
                    }
                    ParentItem.IsSelected = ParentItem.NavigateUrl == $"{Model.WebsiteDomain}{Model.UrlCurrentPage}";

                    var Children = Pages.Where(Item => Item.PageParentID == P1.PageID);
                    ParentItem.Children = new List<ProjectMenuItem>();
                    foreach (var P2 in Children)
                    {
                        var ChildItem = new ProjectMenuItem();
                        ChildItem.Caption = Model.Utilities.GetValuesByLanguage(Model.Culture, P2.PageTitle, P2.PageTitleEng, P2.PageTitleRus);
                        ChildItem.NavigateUrl = Model.GetRouteByName(ControllerActionRouteNames.Website.Pages.Page, new { Culture = Model.Culture, PageSlugHierarchy = P2.PageSlugHierarchy });
                        if (P2.PageIsExternalUrl)
                        {
                            ChildItem.NavigateUrl = P2.PageExternalUrl;
                            if (!string.IsNullOrWhiteSpace(P2.PageExternalUrl))
                            {
                                ChildItem.IsTargetBlank = !P2.PageExternalUrl.Contains(Model.WebsiteDomain) && !P2.PageExternalUrl.StartsWith("/");
                            }
                        }
                        else
                        {
                            ChildItem.NavigateUrl = Model.GetRouteByName(ControllerActionRouteNames.Website.Pages.Page, new { Culture = Model.Culture, PageSlugHierarchy = P2.PageSlugHierarchy });
                        }
                        ChildItem.IsSelected = ChildItem.NavigateUrl == $"{Model.WebsiteDomain}{Model.UrlCurrentPage}";
                        ParentItem.Children.Add(ChildItem);
                    }
                    ViewModel.Menu.Add(ParentItem);
                }
            }
        }

        async Task InitSystemProperties()
        {
            Model.SystemProperties = await Model.DataAccessFactory.SystemProperties.GetSystemProperties();
        }

        void InitLanguageSwitch()
        {
            ViewModel.ShowUrlKa = Model.Culture != Enums.Languages.GEORGIAN;
            ViewModel.ShowUrlEn = !ViewModel.ShowUrlKa;

            var Index = Model.UrlCurrentPage.IndexOf('/', 8) + 1;

            ViewModel.UrlEn = Model.UrlCurrentPage.Replace($"/{Model.Culture}", null);
            ViewModel.UrlKa = Model.UrlCurrentPage.Insert(Index, $"{Enums.Languages.ENGLISH}/").Replace($"/{Model.Culture}", null);
        }
    }
}