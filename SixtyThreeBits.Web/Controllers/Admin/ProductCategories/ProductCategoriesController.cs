using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/product-categories")]
    public class ProductCategoriesController : AdminControllerBase<ProductCategoriesModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.ProductCategoriesController.Categories)]
        public async Task<IActionResult> Categories()
        {
            Model.PluginsClient.EnableSortableJS(true).EnableTemplate7(true);
            var viewModel = await Model.GetViewModel();
            return View(ViewNames.Admin.ProductCategories.ProductCategoriesView, viewModel);
        }

        [Route("add", Name = ControllerActionRouteNames.Admin.ProductCategoriesController.Add)]
        public async Task<IActionResult> Add(ProductCategoriesModel.ProductCategoryCreateSubmitModel submitModel)
        {
            var viewModel = await Model.Add(submitModel);
            return Json(viewModel);
        }

        [Route("sort", Name = ControllerActionRouteNames.Admin.ProductCategoriesController.Sort)]
        public async Task<IActionResult> Sort(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = await Model.Sort(submitModel);
            return Json(viewModel);
        }

        [Route("delete", Name = ControllerActionRouteNames.Admin.ProductCategoriesController.Delete)]
        public async Task<IActionResult> Delete(ProductCategoriesModel.ProductCategoryDeleteSubmitModel submitModel)
        {
            var viewModel = await Model.Delete(submitModel);
            return Json(viewModel);
        }
        #endregion
    }    
}