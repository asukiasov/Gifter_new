using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/brands")]
    public class BrandsController : AdminControllerBase<BrandsModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.BrandsController.Brands)]
        public IActionResult Brands()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.Brands.BrandsView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.BrandsController.Grid)]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.GetGridModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.BrandsController.GridAdd)]
        public async Task<IActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<BrandsModel.ViewModel.GridModel.GridItem>() ?? new BrandsModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.INSERT, brandID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.BrandsController.GridUpdate)]
        public async Task<IActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<BrandsModel.ViewModel.GridModel.GridItem>() ?? new BrandsModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, brandID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.BrandsController.GridDelete)]
        public async Task<IActionResult> GridDelete(int? key)
        {
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.DELETE, brandID: key, submitModel: new BrandsModel.ViewModel.GridModel.GridItem());
            return DevExtremeGridActionResult(viewModel);
        }
        #endregion
    }
}
