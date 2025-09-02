using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/product-categories/{productCategoryID:int}/properties")]
    [TypeFilter(typeof(ProductCategoryFilterAttribute), Order = 2)]
    public class ProductCategoryPropertiesController : AdminControllerBase<ProductCategoryPropertiesModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.ProductCategoryPropertiesController.Properties)]
        public IActionResult Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetViewModel(viewModel: null);
            if (viewModel == null)
            {
                return Model.GetNotFoundAdminViewResult();
            }
            else
            {
                return View(ViewNames.Admin.ProductCategories.ProductCategoryPropertiesView, viewModel);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(ProductCategoryPropertiesModel.ViewModel submitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableDevextreme(true).EnableTinyMce(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = await Model.Save(submitModel);
            if(viewModel.HasFormErrors)
            {
                return View(ViewNames.Admin.ProductCategories.ProductCategoryPropertiesView, viewModel); 
            }
            else
            {
                Model.ShowSuccessToastNotification();
                return Redirect(Model.UrlCurrentPageWithDomain);
            }
        }

        [HttpPost]
        [Route("delete-image", Name = ControllerActionRouteNames.Admin.ProductCategoryPropertiesController.DeleteImage)]
        public async Task<IActionResult> DeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }
        #endregion
    }
}