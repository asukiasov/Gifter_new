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
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.ProductsController.Products)]
        public async Task<ActionResult> Products()
        {
            Model.PluginsClient.EnableDevextreme(true).EnableDevextremeExportExcelLibraries(true).Enable63BitsForms(true).EnableTemplate7(true);
            var viewModel = await Model.GetViewModel();
            return View(ViewNames.Admin.Products.ProductsView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.ProductsController.Grid)]
        public async Task<ActionResult> Grid()
        {
            var viewModel = await Model.ListGridItems();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.ProductsController.GridAdd)]
        public async Task<ActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<ProductsModel.ViewModel.GridViewModel.GridItem>() ?? new ProductsModel.ViewModel.GridViewModel.GridItem();
            await Model.IUD(databaseAction: Enums.DatabaseActions.CREATE, productID: key, submitModel: submitModel);
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
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.ProductsController.GridUpdate)]
        public async Task<ActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<ProductsModel.ViewModel.GridViewModel.GridItem>() ?? new ProductsModel.ViewModel.GridViewModel.GridItem();
            await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, productID: key, submitModel: submitModel);
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
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.ProductsController.GridDelete)]
        public async Task<ActionResult> GridDelete(int? key)
        {
            await Model.IUD(databaseAction: Enums.DatabaseActions.DELETE, productID: key, submitModel: new ProductsModel.ViewModel.GridViewModel.GridItem());
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

        [Route("excel/download", Name = ControllerActionRouteNames.Admin.ProductsController.ExcelDownload)]
        public async Task<IActionResult> ExcelDownload()
        {
            var excelFileBytes = await Model.GetProductsSyncExcelFileBytes();
            return File(excelFileBytes, "application/force-download", "ProductsSync.xlsx");
        }

        [HttpPost]
        [Route("excel/upload", Name = ControllerActionRouteNames.Admin.ProductsController.ExcelUpload)]
        public async Task<IActionResult> ExcelUpload(ProductsModel.ExcelUploadSubmitModel submitModel)
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
            var result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).Enable63BitsFileUploader(true).EnableFancybox(true).EnableTinyMce(true).EnableJQueryNumericInput(true).EnableTemplate7(true).EnableSortableJS(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = await Model.GetViewModel(submitModel);
            Model.Validate(viewModel);
            if (viewModel.IsValid)
            {
                await Model.Save(viewModel);
                if (viewModel.IsValid)
                {
                    Model.ShowSuccessToastNotification();
                    result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.ProductPropertiesController.Properties, new { productID = Model.DBItem.ProductID }));
                }
                else
                {
                    Model.ShowErrorToastNotification();
                    result = View(ViewNames.Admin.Products.ProductPropertiesView, viewModel);
                }
            }
            else
            {
                result = View(ViewNames.Admin.Products.ProductPropertiesView, viewModel);
            }
            return result;
        }

        [HttpPost]
        [Route("images/upload", Name = ControllerActionRouteNames.Admin.ProductPropertiesController.ProductImagesUpload)]
        public async Task<IActionResult> PropertiesImagesUpload()
        {
            var viewModel = await Model.UploadProductImages();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("images/sort", Name = ControllerActionRouteNames.Admin.ProductPropertiesController.ProductImagesSort)]
        public async Task<IActionResult> PropertiesImagesSort(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = await Model.SortProductImages(submitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("images/delete", Name = ControllerActionRouteNames.Admin.ProductPropertiesController.ProductImagesDelete)]
        public async Task<IActionResult> PropertiesImagesDelete(ProductPropertiesModel.DeleteProductImageSubmitModel submitModel)
        {
            var viewModel = await Model.DeleteProductImages(submitModel);
            return Json(viewModel);
        }
        #endregion
    }
}