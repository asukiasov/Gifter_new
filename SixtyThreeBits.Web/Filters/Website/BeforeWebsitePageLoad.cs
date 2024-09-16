using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Domain.ViewModels.Website;
using SixtyThreeBits.Web.Models.Base;
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
            _model = WebUtilities.GetModelFromController<ModelBase>(filterContext.Controller);
            var c = filterContext.Controller as Controller;


            initStartUp(filterContext);
            if (!_model.IsAjaxRequest)
            {
                var redirectResult = await checkRedirect();
                if (redirectResult.IsRedirect)
                {
                    filterContext.Result = new RedirectResult(redirectResult.RedirectUrl, permanent: true);
                }
                else
                {
                    initViewModel();
                    initClientPlugins();
                    initPageTitle();
                    await initMenu();

                    WebUtilities.SetLayoutViewModel(viewData: c.ViewData, viewModel: _viewModel, key: WebConstants.ViewData.LayoutViewModel);
                }
            }
            await next();
        }

        void initStartUp(ActionExecutingContext filterContext)
        {
            _model.LanguageCultureCode = filterContext.RouteData.Values[WebConstants.RouteValues.Culture]?.ToString() ?? _model.Utilities.LanguageDefault.LanguageCultureCode;
        }

        void initViewModel()
        {
            _viewModel = new WebsiteLayoutViewModel();
            _viewModel.ProjectName = _model.SystemProperties.ProjectName;
            _viewModel.ContactPhone = _model.SystemProperties.ContactPhone;
            _viewModel.ContactEmail = _model.SystemProperties.ContactEmail;
            _viewModel.ContactAddress = _model.Utilities.GetValuesByLanguage(
                culture: _model.LanguageCultureCode,
                georgianValue: _model.SystemProperties.ContactAddress,
                englishValue: _model.SystemProperties.ContactAddressEng
            );
            _viewModel.FacebookUrl = _model.SystemProperties.FacebookUrl;
            _viewModel.InstagramUrl = _model.SystemProperties.InstagramUrl;
            _viewModel.TwitterUrl = _model.SystemProperties.TwitterUrl;
            _viewModel.YoutubeUrl = _model.SystemProperties.YoutubeUrl;
            _viewModel.LinkedInUrl = _model.SystemProperties.LinkedInUrl;
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
            .EnableUtils(true)
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
            var repositoryMenuHeader = _model.RepositoriesFactory.CreateMenuHeaderRepository();
            var menuHeader = await repositoryMenuHeader.MenuHeaderList(menuHeaderIsPublished: true);

            if(menuHeader!= null)
            {
                _viewModel.Menu = new List<ProjectMenuViewItem>();
                var parents = menuHeader.Where(item => item.MenuHeaderParentID == null);
                foreach (var parent in parents)
                {
                    var parentItem = new ProjectMenuViewItem();
                    if(parent.MenuHeaderIsExternalPage)
                    {
                        parentItem.Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, parent.MenuHeaderTitle, parent.MenuHeaderTitleEng);
                        parentItem.NavigateUrl = parent.MenuHeaderExternalPageUrl;
                    }
                    else
                    {
                        parentItem.Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, parent.PageTitle, parent.PageTitleEng);
                        parentItem.NavigateUrl = _model.GetRouteByName(ControllerActionRouteNames.Website.PagesController.Page, new { pageSlug = parent.PageSlug });
                    }

                    var children = menuHeader.Where(item => item.MenuHeaderParentID == parent.MenuHeaderID);
                    if (children.Any())
                    {
                        parentItem.Children = new List<ProjectMenuViewItem>();

                        foreach (var child in children)
                        {
                            var childItem = new ProjectMenuViewItem();
                            if (child.MenuHeaderIsExternalPage)
                            {
                                childItem.Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, child.MenuHeaderTitle, child.MenuHeaderTitleEng);
                                childItem.NavigateUrl = childItem.NavigateUrl;
                            }
                            else
                            {
                                childItem.Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, child.PageTitle, child.PageTitleEng);
                                childItem.NavigateUrl = _model.GetRouteByName(ControllerActionRouteNames.Website.PagesController.Page, new { pageSlug = child.PageSlug });
                            }

                            _viewModel.Menu.Add(childItem);
                        }
                    }
                    
                    _viewModel.Menu.Add(parentItem);
                }
            }

            var repositoryMenuFooter = _model.RepositoriesFactory.CreateMenuFooterRepository();
            var menuFooter = await repositoryMenuFooter.MenuFooterList(menuFooterIsPublished: true);
            if(menuFooter?.Any() == true)
            {
                var menuFooterColumn1 = menuFooter.Take(4);
                var menuFooterColumn2 = menuFooter.Skip(4).Take(4);
                _viewModel.FooterMenu1 = menuFooterColumn1.Select((item) => 
                {
                    var footerItem = new ProjectMenuViewItem();
                    if (item.MenuFooterIsExternalPage)
                    {
                        footerItem.Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, item.MenuFooterTitle, item.MenuFooterTitleEng);
                        footerItem.NavigateUrl = item.MenuFooterExternalPageUrl;
                    }
                    else
                    {
                        footerItem.Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, item.PageTitle, item.PageTitleEng);
                        footerItem.NavigateUrl = _model.GetRouteByName(ControllerActionRouteNames.Website.PagesController.Page, new { pageSlug = item.PageSlug });
                    }
                    return footerItem;
                }).ToList();
                _viewModel.FooterMenu2 = menuFooterColumn2.Select((item) =>
                {
                    var footerItem = new ProjectMenuViewItem();
                    if (item.MenuFooterIsExternalPage)
                    {
                        footerItem.Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, item.MenuFooterTitle, item.MenuFooterTitleEng);
                        footerItem.NavigateUrl = item.MenuFooterExternalPageUrl;
                    }
                    else
                    {
                        footerItem.Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, item.PageTitle, item.PageTitleEng);
                        footerItem.NavigateUrl = _model.GetRouteByName(ControllerActionRouteNames.Website.PagesController.Page, new { pageSlug = item.PageSlug });
                    }
                    return footerItem;
                }).ToList();
            }
        }

        async Task<customRedirectResult> checkRedirect()
        {
            var result = new customRedirectResult();
            var repository = _model.RepositoriesFactory.CreateRedirectsRepository();
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