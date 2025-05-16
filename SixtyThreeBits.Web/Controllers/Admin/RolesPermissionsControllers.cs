using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/roles-permissions")]
    public class RolePermissionsController : AdminControllerBase<RolePermissionsModel>
    {
        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.RolePermissionsController.RolesPermissions)]
        public ActionResult RolesPermissions()
        {
            Model.PluginsClient.EnableDevextreme(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.RolesPermissions.RolesPermissionsView, viewModel);
        }
        
        [HttpGet]
        [Route("roles/grid", Name = ControllerActionRouteNames.Admin.RolePermissionsController.RolesGrid)]
        public async Task<ActionResult> RolesGrid()
        {
            var viewModel = await Model.ListRolesGridItems();
            return Json(viewModel);
        }

        [HttpGet]
        [Route("permissions/tree", Name = ControllerActionRouteNames.Admin.RolePermissionsController.PermissionsTree)]
        public async Task<ActionResult> PermissionsTree()
        {
            var viewModel = await Model.ListPermissionsTreeItems();
            return Json(viewModel);
        }

        [HttpGet]
        [Route("permissions/get-by-role", Name = ControllerActionRouteNames.Admin.RolePermissionsController.GetPermissionsByRole)]
        public async Task<ActionResult> PermissionsGetByRole(int? RoleID)
        {
            var viewModel = await Model.GetRolePermissions(RoleID);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("save", Name = ControllerActionRouteNames.Admin.RolePermissionsController.Save)]
        public async Task<ActionResult> Save(RolePermissionsModel.SubmitModelRolePermissionSave submitModel)
        {
            var viewModel = await Model.Save(submitModel);
            return Json(viewModel);
        }
        #endregion
    }
}