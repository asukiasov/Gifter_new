using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/products")]
    public class ProductsController : AdminControllerBase<ProductsModel>
    {
        #region Constructors
        public ProductsController()
        {
            Model = new ProductsModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Products.Index)]
        public async Task<ActionResult> Products()
        {
            Model.PluginsClient.EnableDevextreme(true).EnableDevextremeExportExcelLibraries(true).Enable63BitsForms(true).EnableTemplate7(true);
            var viewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.Products.Page, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Products.Grid)]
        public async Task<ActionResult> ProductsGrid()
        {
            var viewModel = await Model.GetGridViewModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Products.GridAdd)]
        public async Task<ActionResult> ProductsGridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<ProductsModel.PageViewModel.GridModel.GridItem>() ?? new ProductsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.CREATE, productID: key, submitModel: submitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.Products.GridUpdate)]
        public async Task<ActionResult> ProductsGridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<ProductsModel.PageViewModel.GridModel.GridItem>() ?? new ProductsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.UPDATE, productID: key, submitModel: submitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.Products.GridDelete)]
        public async Task<ActionResult> ProductsGridDelete(int? key)
        {
            await Model.CRUD(databaseAction: Enums.DatabaseActions.DELETE, productID: key, submitModel: new ProductsModel.PageViewModel.GridModel.GridItem());
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

        [Route("excel/download", Name = ControllerActionRouteNames.Admin.Products.ExcelDownload)]
        public async Task<IActionResult> ExcelDownload()
        {
            var excelFileBytes = await Model.GetProductsSyncExcelFileBytes();
            return File(excelFileBytes, "application/force-download", "ProductsSync.xlsx");
        }

        [HttpPost]
        [Route("excel/upload", Name = ControllerActionRouteNames.Admin.Products.ExcelUpload)]
        public async Task<IActionResult> ExcelSync(ProductsModel.ExcelUploadSubmitModel submitModel)
        {
            var viewModel = await Model.SyncExcel(submitModel);
            return Json(viewModel);
        }
        #endregion
    }

    [Route("admin/products/{productID:int}/properties")]
    [TypeFilter(typeof(BeforeProductPageLoad), Order = 2)]
    public class ProductsPropertiesController : AdminControllerBase<ProductPropertiesModel>
    {
        #region Constructors
        public ProductsPropertiesController()
        {
            Model = new ProductPropertiesModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Products.Product.Properties)]
        public async Task<IActionResult> Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).Enable63BitsFileUploader(true).EnableFancybox(true).EnableTinyMce(true).EnableJQueryNumericInput(true).EnableTemplate7(true).EnableSortableJS(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.Products.ProductProperties, viewModel);

        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(ProductPropertiesModel.ProductsPropertiesViewModel submitModel)
        {
            var result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).Enable63BitsFileUploader(true).EnableFancybox(true).EnableTinyMce(true).EnableJQueryNumericInput(true).EnableTemplate7(true).EnableSortableJS(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = await Model.GetPageViewModel(submitModel);
            Model.ValidatePageViewModel(viewModel);
            if (viewModel.IsValid)
            {
                await Model.Save(viewModel);
                if (viewModel.IsSaved)
                {
                    Model.ShowSuccessToastNotification();
                    result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Products.Product.Properties, new { Model.DBItem.ProductID }));
                }
                else
                {
                    Model.ShowErrorToastNotification();
                    result = View(ViewNames.Admin.Products.ProductProperties, viewModel);
                }
            }
            else
            {
                result = View(ViewNames.Admin.Products.ProductProperties, viewModel);
            }
            return result;
        }

        [HttpPost]
        [Route("images/upload", Name = ControllerActionRouteNames.Admin.Products.Product.PropertiesImagesUpload)]
        public async Task<IActionResult> PropertiesImagesUpload()
        {
            var viewModel = await Model.UploadImages();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("images/sort", Name = ControllerActionRouteNames.Admin.Products.Product.PropertiesImagesSort)]
        public async Task<IActionResult> PropertiesImagesSort(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = await Model.SortImages(submitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("images/delete", Name = ControllerActionRouteNames.Admin.Products.Product.PropertiesImagesDelete)]
        public async Task<IActionResult> PropertiesImagesDelete(ProductPropertiesModel.DeleteImageSubmitModel submitModel)
        {
            var viewModel = await Model.DeleteImage(submitModel);
            return Json(viewModel);
        }
        #endregion
    }
}