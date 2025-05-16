using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class PagesController : WebsiteControllerBase<PagesModel>
    {
        #region Actions
        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-5.0#rtr        
        [Route($"{RouteValueLanguageCode}/{{*pageSlug}}", Name = ControllerActionRouteNames.Website.PagesController.PageCulture, Order = 9999)]
        [Route("{*pageSlug}", Name = ControllerActionRouteNames.Website.PagesController.Page, Order = 10000)]        
        public async Task<IActionResult> Page(string pageSlug)
        {
            Model.PluginsClient.EnablePageBuilder(true).EnableSlickSlider(true).EnableJQueryAppear(true).EnableJWPlayer(true);
            var viewModel = await Model.GetViewModel(pageSlug);
            if (viewModel == null)
            {
                return Model.GetNotFoundWebsiteViewResult();
            }
            else
            {
                Model.PageTitle.Set(viewModel.PageTitle);
                return View(ViewNames.Website.Pages.PageView, viewModel);
            }
        }         
        #endregion
    }
}