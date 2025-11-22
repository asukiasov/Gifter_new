using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/permissions")]
    public class PermissionsController : AdminControllerBase<PermissionsModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.PermissionsController.Permissions)]
        public IActionResult Permissions()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.Permissions.PermissionsView, viewModel);
        }

        [Route("tree", Name = ControllerActionRouteNames.Admin.PermissionsController.Tree)]
        public async Task<IActionResult> Tree()
        {
            var viewModel = await Model.GetTreeItems();
            return DevExtremeGridResult(viewModel);
        }

        [HttpPost]
        [Route("tree/add", Name = ControllerActionRouteNames.Admin.PermissionsController.TreeAdd)]
        public async Task<IActionResult> TreeAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<PermissionsModel.ViewModel.TreeModel.TreeItem>() ?? new PermissionsModel.ViewModel.TreeModel.TreeItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.INSERT, permissionID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpPut]
        [Route("tree/update", Name = ControllerActionRouteNames.Admin.PermissionsController.TreeUpdate)]
        public async Task<IActionResult> TreeUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<PermissionsModel.ViewModel.TreeModel.TreeItem>() ?? new PermissionsModel.ViewModel.TreeModel.TreeItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, permissionID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpDelete]
        [Route("tree/delete", Name = ControllerActionRouteNames.Admin.PermissionsController.TreeDelete)]
        public async Task<IActionResult> TreeDelete(int? key)
        {
            var viewModel = await Model.DeleteRecursive(permissionID: key);
            return DevExtremeGridActionResult(viewModel);
        }
        #endregion
    }    
}