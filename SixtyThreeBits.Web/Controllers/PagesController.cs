using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers
{
    public class PagesController : WebsiteControllerBase<PagesModel>
    {
        public PagesController()
        {
            Model = new PagesModel();
        }

        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-5.0#rtr        
        [Route("{*PageSlugHierarchy}", Name = ControllerActionRouteNames.Website.Pages.Page, Order = 9999)]
        [Route("{Culture:regex(ka|en|ru)}/{*PageSlugHierarchy}", Name = ControllerActionRouteNames.Website.Pages.PageCulture, Order = 10000)]
        public async Task<IActionResult> Page(string PageSlugHierarchy)
        {
            Model.PluginsClient.EnablePageBuilder(true).EnableSlickSlider(true).EnableJQueryAppear(true).EnableJWPlayer(true);
            var ViewModel = await Model.GetPageViewModel(PageSlugHierarchy);
            if (ViewModel == null)
            {
                return NotFound();
            }
            else
            {
                Model.PageTitle.Set(ViewModel.PageTitle);
                return View(ViewNames.Website.Pages.Page, ViewModel);
            }
        }

        [Route("error/404/")]
        public IActionResult Error()
        {
            return View(ViewNames.Shared.NotFound);
        }
    }
}