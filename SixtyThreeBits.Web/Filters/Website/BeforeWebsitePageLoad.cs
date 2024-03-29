using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using SixtyThreeBits.Web.Models.Shared;
using SixtyThreeBits.Web.Models.Website;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SixtyThreeBits.Web.Filters.Website
{
    public class BeforeWebsitePageLoad : IAsyncActionFilter
    {
        #region Properties
        ModelBase _model;
        WebsiteLayoutViewModel _viewModel;
        #endregion

        #region Methods
        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _viewModel = new WebsiteLayoutViewModel();
            _model = WebUtilities.GetModelFromController<ModelBase>(filterContext.Controller);
            var c = filterContext.Controller as Controller;

            var redirectResult = await checkRedirect();
            if (redirectResult.IsRedirect)
            {
                filterContext.Result = new RedirectResult(redirectResult.RedirectUrl, permanent: true);
            }
            else
            {
                await initStartUp(filterContext);
                if (!_model.IsAjaxRequest)
                {
                    initClientPlugins();
                    initPageTitle();
                    await initMenu();
                    initLanguageSwitch();

                    WebUtilities.SetLayoutViewModel(viewData: c.ViewData, viewModel: _viewModel, key: WebConstants.ViewData.LayoutViewModel);
                }
                await next();
            }
        }

        async Task initStartUp(ActionExecutingContext filterContext)
        {
            var repository = _model.RepositoriesFactory.GetSystemPropertiesRepository();
            _model.LanguageCultureCode = filterContext.RouteData.Values[WebConstants.RouteValues.Culture]?.ToString() ?? Enums.Languages.GEORGIAN;
            _model.SystemProperties = await repository.SystemPropertiesGet();
            _viewModel.ScriptsHeader = _model.SystemProperties.ScriptsHeader;
            _viewModel.ScriptsBodyStart = _model.SystemProperties.ScriptsBodyStart;
            _viewModel.ScriptsBodyEnd = _model.SystemProperties.ScriptsBodyEnd;
        }

        void initClientPlugins()
        {
            _model.PluginsClient
            .EnableGoogleFonts(true)
            .EnableFontAwesome(true)
            .Enable63BitsFonts(true)
            .EnableBootstrap(true)
            .EnableJQuery(true)
            .EnablePreloader(true);

            _viewModel.PluginsClient = _model.PluginsClient;
        }

        void initPageTitle()
        {
            _model.PageTitle = _viewModel.PageTitle = new PageTitle(_model.SystemProperties.ProjectName);
        }

        async Task initMenu()
        {
            var repository = _model.RepositoriesFactory.GetPagesRepository();
            var pages = await repository.PagesList(pageIsPublished: true, pageIsMenuItem: true);
            if (pages != null)
            {
                _viewModel.Menu = new List<ProjectMenuItem>();
                var parents = pages.Where(item => item.PageParentID == null);
                foreach (var p1 in parents)
                {
                    var parentItem = new ProjectMenuItem();
                    parentItem.Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, p1.PageTitle, p1.PageTitleEng);
                    if (p1.PageIsExternalUrl)
                    {
                        parentItem.NavigateUrl = p1.PageExternalUrl;
                        if (!string.IsNullOrWhiteSpace(p1.PageExternalUrl))
                        {
                            parentItem.IsTargetBlank = !p1.PageExternalUrl.Contains(_model.WebsiteDomain) && !p1.PageExternalUrl.StartsWith("/");
                        }
                    }
                    else
                    {
                        parentItem.NavigateUrl = _model.GetRouteByName(ControllerActionRouteNames.Website.Pages.Page, new { culture = _model.LanguageCultureCode, pageSlugHierarchy = p1.PageSlugHierarchy }); ;
                    }
                    parentItem.IsSelected = parentItem.NavigateUrl == $"{_model.WebsiteDomain}{_model.UrlCurrentPageWithDomain}";

                    var children = pages.Where(item => item.PageParentID == p1.PageID);
                    parentItem.Children = new List<ProjectMenuItem>();
                    foreach (var p2 in children)
                    {
                        var childItem = new ProjectMenuItem();
                        childItem.Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, p2.PageTitle, p2.PageTitleEng);
                        childItem.NavigateUrl = _model.GetRouteByName(ControllerActionRouteNames.Website.Pages.Page, new { culture = _model.LanguageCultureCode, pageSlugHierarchy = p2.PageSlugHierarchy });
                        if (p2.PageIsExternalUrl)
                        {
                            childItem.NavigateUrl = p2.PageExternalUrl;
                            if (!string.IsNullOrWhiteSpace(p2.PageExternalUrl))
                            {
                                childItem.IsTargetBlank = !p2.PageExternalUrl.Contains(_model.WebsiteDomain) && !p2.PageExternalUrl.StartsWith("/");
                            }
                        }
                        else
                        {
                            childItem.NavigateUrl = _model.GetRouteByName(ControllerActionRouteNames.Website.Pages.Page, new { Culture = _model.LanguageCultureCode, p2.PageSlugHierarchy });
                        }
                        childItem.IsSelected = childItem.NavigateUrl == $"{_model.WebsiteDomain}{_model.UrlCurrentPageWithDomain}";
                        parentItem.Children.Add(childItem);
                    }
                    _viewModel.Menu.Add(parentItem);
                }
            }
        }

        void initLanguageSwitch()
        {
            _viewModel.ShowUrlKa = _model.LanguageCultureCode != Enums.Languages.GEORGIAN;
            _viewModel.ShowUrlEn = !_viewModel.ShowUrlKa;

            var Index = _model.UrlCurrentPageWithDomain.IndexOf('/', 8) + 1;

            _viewModel.UrlEn = _model.UrlCurrentPageWithDomain.Replace($"/{_model.LanguageCultureCode}", null);
            _viewModel.UrlKa = _model.UrlCurrentPageWithDomain.Insert(Index, $"{Enums.Languages.ENGLISH}/").Replace($"/{_model.LanguageCultureCode}", null);
        }

        async Task<customRedirectResult> checkRedirect()
        {
            var result = new customRedirectResult();
            var repository = _model.RepositoriesFactory.GetRedirectsRepository();
            var redirects = await repository.RedirectsList();
            if (redirects?.Any() == true)
            {
                var pathToCompare = HttpUtility.UrlDecode(_model.UrlCurrentPageWithoutDomain.Trim('/'));
                var found = redirects.FirstOrDefault(Item => Item.RedirectFrom == pathToCompare);
                if (found != null)
                {
                    var redirectUrl = string.IsNullOrWhiteSpace(found.RedirectTo) || found.RedirectTo == "/" ? _model.WebsiteDomain : found.RedirectTo;
                    result.IsRedirect = true;
                    result.RedirectUrl = redirectUrl;
                }
            }
            return result;
        }
        #endregion

        #region Nested Classes
        class customRedirectResult
        {
            #region Properties
            public bool IsRedirect { get; set; }
            public string RedirectUrl { get; set; }
            #endregion
        }
        #endregion
    }
}