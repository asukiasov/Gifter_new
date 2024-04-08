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
        #region Constructors
        public ProductCategoriesController()
        {
            Model = new ProductsCategoriesModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.ProductCategories.Index)]
        public async Task<IActionResult> Categories()
        {
            Model.PluginsClient.EnableSortableJS(true).EnableTemplate7(true);
            var viewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.ProductCategories.Page, viewModel);
        }

        [Route("add", Name = ControllerActionRouteNames.Admin.ProductCategories.Add)]
        public async Task<IActionResult> Create(ProductsCategoriesModel.ProductCategoryCreateSubmitModel submitModel)
        {
            var viewModel = await Model.CreateProductCategory(submitModel);
            return Json(viewModel);
        }

        [Route("sort", Name = ControllerActionRouteNames.Admin.ProductCategories.Sort)]
        public async Task<IActionResult> Sort(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = await Model.SyncParentsAndSortIndexes(submitModel);
            return Json(viewModel);
        }

        [Route("delete", Name = ControllerActionRouteNames.Admin.ProductCategories.Delete)]
        public async Task<IActionResult> Delete(ProductsCategoriesModel.ProductCategoryDeleteSubmitModel submitModel)
        {
            var viewModel = await Model.DeleteRecursive(submitModel);
            return Json(viewModel);
        }
        #endregion
    }

    [Route("admin/product-categories/{productCategoryID:int}/properties")]
    [TypeFilter(typeof(BeforeProductCategoryPageLoad), Order = 2)]
    public class ProductCategoryPropertiesController : AdminControllerBase<ProductCategoryPropertiesModel>
    {
        #region Constructors
        public ProductCategoryPropertiesController()
        {
            Model = new ProductCategoryPropertiesModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.ProductCategories.ProductCategory.Properties)]
        public IActionResult Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetPageViewModel(viewModel: null);
            if (viewModel == null)
            {
                return Model.GetNotFoundAdminViewResult();
            }
            else
            {
                return View(ViewNames.Admin.ProductCategories.ProductCategoryProperties, viewModel);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(ProductCategoryPropertiesModel.ProductCategoryPropertiesViewModel submitModel)
        {
            var result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableDevextreme(true).EnableTinyMce(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetPageViewModel(submitModel);
            Model.ValidatePageViewModel(viewModel);
            if (viewModel.IsValid)
            {
                await Model.Save(viewModel);
                if (viewModel.IsSaved)
                {
                    Model.ShowSuccessToastNotification();
                    result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.ProductCategories.ProductCategory.Properties, new { productCategoryID = Model.DBItem.ProductCategoryID }));
                }
                else
                {
                    Model.ShowErrorToastNotification();
                    result = View(ViewNames.Admin.ProductCategories.ProductCategoryProperties, viewModel);
                }
            }
            else
            {
                result = View(ViewNames.Admin.ProductCategories.ProductCategoryProperties, viewModel);
            }
            return result;
        }

        [HttpPost]
        [Route("image/delete", Name = ControllerActionRouteNames.Admin.ProductCategories.ProductCategory.ImageDelete)]
        public async Task<IActionResult> CategoryDeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }
        #endregion
    }
}