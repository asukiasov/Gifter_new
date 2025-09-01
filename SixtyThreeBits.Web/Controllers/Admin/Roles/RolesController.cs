using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/roles")]
    public class RolesController : AdminControllerBase<RolesModel>
    {
        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.RolesController.Roles)]
        public IActionResult Roles()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.Roles.RolesView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.RolesController.Grid)]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.GetGridItems();
            return DevExtremeGridResult(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.RolesController.GridAdd)]
        public async Task<IActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<RolesModel.ViewModel.GridModel.GridItem>() ?? new RolesModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.INSERT, roleID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.RolesController.GridUpdate)]
        public async Task<IActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<RolesModel.ViewModel.GridModel.GridItem>() ?? new RolesModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, roleID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);            
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.RolesController.GridDelete)]
        public async Task<IActionResult> GridDelete(int? key)
        {
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.DELETE, roleID: key, submitModel: new RolesModel.ViewModel.GridModel.GridItem());
            return DevExtremeGridActionResult(viewModel);
        }
        #endregion
    }    
}