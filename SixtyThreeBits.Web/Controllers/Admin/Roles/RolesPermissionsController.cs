using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/roles-permissions")]
    public class RolesPermissionsController : AdminControllerBase<RolesPermissionsModel>
    {
        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.RolePermissionsController.RolesPermissions)]
        public IActionResult RolesPermissions()
        {
            Model.PluginsClient.EnableDevextreme(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.RolesPermissions.RolesPermissionsView, viewModel);
        }
        
        [HttpGet]
        [Route("roles/grid", Name = ControllerActionRouteNames.Admin.RolePermissionsController.RolesGrid)]
        public async Task<IActionResult> RolesGrid()
        {
            var viewModel = await Model.GetRolesGridItems();
            return DevExtremeGridResult(viewModel);
        }

        [HttpGet]
        [Route("permissions/tree", Name = ControllerActionRouteNames.Admin.RolePermissionsController.PermissionsTree)]
        public async Task<IActionResult> PermissionsTree()
        {
            var viewModel = await Model.GetPermissionsTreeItems();
            return DevExtremeGridResult(viewModel);
        }

        [HttpGet]
        [Route("permissions/get-by-role", Name = ControllerActionRouteNames.Admin.RolePermissionsController.GetPermissionsByRole)]
        public async Task<IActionResult> PermissionsGetByRole(int? RoleID)
        {
            var viewModel = await Model.GetRolePermissions(RoleID);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("save", Name = ControllerActionRouteNames.Admin.RolePermissionsController.Save)]
        public async Task<IActionResult> Save(RolesPermissionsModel.SubmitModelRolePermissionSave submitModel)
        {
            var viewModel = await Model.Save(submitModel);
            return Json(viewModel);
        }
        #endregion
    }
}