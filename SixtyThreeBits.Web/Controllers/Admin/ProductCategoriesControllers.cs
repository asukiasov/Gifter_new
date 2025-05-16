using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/product-categories")]
    public class ProductCategoriesController : AdminControllerBase<ProductsCategoriesModel>
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
        public async Task<IActionResult> Add(ProductsCategoriesModel.ProductCategoryCreateSubmitModel submitModel)
        {
            var viewModel = await Model.CreateProductCategory(submitModel);
            return Json(viewModel);
        }

        [Route("sort", Name = ControllerActionRouteNames.Admin.ProductCategoriesController.Sort)]
        public async Task<IActionResult> Sort(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = await Model.SyncParentsAndSortIndexes(submitModel);
            return Json(viewModel);
        }

        [Route("delete", Name = ControllerActionRouteNames.Admin.ProductCategoriesController.Delete)]
        public async Task<IActionResult> Delete(ProductsCategoriesModel.ProductCategoryDeleteSubmitModel submitModel)
        {
            var viewModel = await Model.DeleteRecursive(submitModel);
            return Json(viewModel);
        }
        #endregion
    }

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
            var result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableDevextreme(true).EnableTinyMce(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetViewModel(submitModel);
            Model.Validate(viewModel);
            if (viewModel.IsValid)
            {
                await Model.Save(viewModel);
                if (viewModel.IsValid)
                {
                    Model.ShowSuccessToastNotification();
                    result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.ProductCategoryPropertiesController.Properties, new { productCategoryID = Model.DBItem.ProductCategoryID }));
                }
                else
                {
                    Model.ShowErrorToastNotification();
                    result = View(ViewNames.Admin.ProductCategories.ProductCategoryPropertiesView, viewModel);
                }
            }
            else
            {
                result = View(ViewNames.Admin.ProductCategories.ProductCategoryPropertiesView, viewModel);
            }
            return result;
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