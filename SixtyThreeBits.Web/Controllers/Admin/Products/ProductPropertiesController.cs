using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/products/{productID:int}/properties")]
    [TypeFilter(typeof(ProductFilterAttribute), Order = 2)]
    public class ProductPropertiesController : AdminControllerBase<ProductPropertiesModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.ProductPropertiesController.Properties)]
        public async Task<IActionResult> Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).Enable63BitsFileUploader(true).EnableFancybox(true).EnableTinyMce(true).EnableJQueryNumericInput(true).EnableTemplate7(true).EnableSortableJS(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = await Model.GetViewModel();
            return View(ViewNames.Admin.Products.ProductPropertiesView, viewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(ProductPropertiesModel.ViewModel submitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true).Enable63BitsFileUploader(true).EnableFancybox(true).EnableTinyMce(true).EnableJQueryNumericInput(true).EnableTemplate7(true).EnableSortableJS(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = await Model.Save(submitModel);

            if (viewModel.HasErrors)
            {
                return View(ViewNames.Admin.Products.ProductPropertiesView, viewModel);
            }
            else
            {
                return Redirect(Model.UrlCurrentPageWithDomain);
            }
        }

        [HttpPost]
        [Route("images/upload", Name = ControllerActionRouteNames.Admin.ProductPropertiesController.ImagesUpload)]
        public async Task<IActionResult> ImagesUpload()
        {
            var viewModel = await Model.UploadProductImages();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("images/update", Name = ControllerActionRouteNames.Admin.ProductPropertiesController.ImagesUpdate)]
        public async Task<IActionResult> ImagesUpdate(ProductPropertiesModel.UpdateProductImageSubmitModel submitModel)
        {
            var viewModel = await Model.UpdateProductImages(submitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("images/delete", Name = ControllerActionRouteNames.Admin.ProductPropertiesController.ImagesDelete)]
        public async Task<IActionResult> ImagesDelete(ProductPropertiesModel.DeleteProductImageSubmitModel submitModel)
        {
            var viewModel = await Model.DeleteProductImages(submitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("images/sort", Name = ControllerActionRouteNames.Admin.ProductPropertiesController.ImagesSort)]
        public async Task<IActionResult> ImagesSort(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = await Model.SortProductImages(submitModel);
            return Json(viewModel);
        }
        #endregion
    }
}