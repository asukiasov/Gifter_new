using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
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
        public async Task<IActionResult> Products()
        {
            Model.PluginsClient.EnableDevextreme(true).EnableDevextremeExportExcelLibraries(true).Enable63BitsForms(true).EnableTemplate7(true);
            var viewModel = await Model.GetViewModel();
            return View(ViewNames.Admin.Products.ProductsView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.ProductsController.Grid)]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.GetGridModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.ProductsController.GridAdd)]
        public async Task<IActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<ProductsModel.ViewModel.GridModel.GridItem>() ?? new ProductsModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.INSERT, productID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);            
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.ProductsController.GridUpdate)]
        public async Task<IActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<ProductsModel.ViewModel.GridModel.GridItem>() ?? new ProductsModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, productID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.ProductsController.GridDelete)]
        public async Task<IActionResult> GridDelete(int? key)
        {
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.DELETE, productID: key, submitModel: new ProductsModel.ViewModel.GridModel.GridItem());
            return DevExtremeGridActionResult(viewModel);
        }

        [Route("excel/download", Name = ControllerActionRouteNames.Admin.ProductsController.ExcelDownload)]
        public async Task<IActionResult> ExcelDownload()
        {
            var viewModel = await Model.GetProductsSyncExcelFileBytes();
            if (viewModel == null)
            {
                return Model.GetNotFoundAdminViewResult();
            }
            else
            {
                return File(viewModel, "application/force-download", "ProductsSync.xlsx");
            }
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
}