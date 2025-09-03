using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/brands/{brandID:int}/properties")]
    [TypeFilter(typeof(BrandFilterAttribute), Order = 2)]
    public class BrandPropertiesController : AdminControllerBase<BrandPropertiesModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.BrandPropertiesController.Properties)]
        public IActionResult Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetViewModel(viewModel: null);
            return View(ViewNames.Admin.Brands.BrandPropertiesView, viewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(BrandPropertiesModel.ViewModel submitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = await Model.Save(submitModel);
            if(viewModel.HasErrors)
            {
                return View(ViewNames.Admin.Brands.BrandPropertiesView, viewModel);
            }
            else
            {
                Model.ShowSuccessToastNotification();
                return Redirect(Model.UrlCurrentPageWithDomain);
            }
        }

        [HttpPost]
        [Route("delete-image", Name = ControllerActionRouteNames.Admin.BrandPropertiesController.DeleteImage)]
        public async Task<IActionResult> DeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }
        #endregion
    }
}
