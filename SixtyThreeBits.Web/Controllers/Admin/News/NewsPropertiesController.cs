using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/news/{newsID:int}")]
    [TypeFilter(typeof(NewsFilterAttribute), Order = 2)]
    public class NewsPropertiesController : AdminControllerBase<NewsPropertiesModel>
    {
        #region Actions
        [HttpGet]
        [Route("properties", Name = ControllerActionRouteNames.Admin.NewsPropertiesController.Properties)]
        public IActionResult Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).EnableDevextreme(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetViewModel(viewModel: null);
            return View(ViewNames.Admin.News.NewsPropertiesView, viewModel);
        }

        [HttpPost]
        [Route("properties")]
        public async Task<IActionResult> Properties(NewsPropertiesModel.ViewModel submitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).EnableDevextreme(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = await Model.Save(submitModel);
            if (viewModel.HasErrors)
            {
                return View(ViewNames.Admin.News.NewsPropertiesView, viewModel);
            }
            else
            {
                Model.ShowSuccessToastNotification();
                return Redirect(Model.UrlCurrentPageWithDomain);
            }
        }

        [HttpPost]
        [Route("properties/delete-image", Name = ControllerActionRouteNames.Admin.NewsPropertiesController.DeleteImage)]
        public async Task<IActionResult> DeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }
        #endregion
    }
}
