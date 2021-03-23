using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers
{
    public class HomeController : WebsiteControllerBase<HomeModel>
    {
        public HomeController()
        {
            Model = new HomeModel();
        }

        [Route("", Name = ControllerActionRouteNames.Website.Home.Page)]
        [Route("{Culture:length(2)}", Name = ControllerActionRouteNames.Website.Home.PageCulture)]
        public IActionResult Index()
        {
            return View(ViewNames.Website.Home.Page);
        }

        [Route("{PageSlug:regex(^(?!admin|sitemap)[[A-Za-z0-9,-]]{{3,}}$)}", Name = ControllerActionRouteNames.Website.Home.StaticPage, Order = 99999)]
        [Route("{Culture:length(2)}/{PageSlug:regex(^(?!admin|sitemap)[[A-Za-z0-9,-]]{{3,}}$)}", Order = 100000)]
        public async Task<IActionResult> StaticPage(string PageSlug)
        {
            Model.PluginsClient.EnablePageBuilder(true).EnableSlickSlider(true).EnableJQueryAppear(true).EnableJWPlayer(true);
            var ViewModel = await Model.GetStaticPageViewModel(PageSlug);
            if (ViewModel == null)
            {
                return NotFound();
            }
            else
            {
                Model.PageTitle.Set(ViewModel.PageTitle);
                return View(ViewNames.Website.Home.StaticPage, ViewModel);
            }
        }

        [Route("error/404/")]
        public IActionResult Error()
        {
            return View(ViewNames.Shared.NotFound);
        }
    }
}