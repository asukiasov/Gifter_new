using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/roles-permissions")]
    public class RolePermissionsController : AdminControllerBase<RolePermissionsModel>
    {
        #region Constructors
        public RolePermissionsController()
        {
            Model = new RolePermissionsModel();
        }
        #endregion

        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.RolesPermissions.Page)]
        public ActionResult RolesPermissions()
        {
            Model.PluginsClient.EnableDevextreme(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.RolesPermissions.Page, viewModel);
        }
        
        [HttpGet]
        [Route("roles/grid", Name = ControllerActionRouteNames.Admin.RolesPermissions.RolesGrid)]
        public async Task<ActionResult> RolesGrid()
        {
            var viewModel = await Model.GetRolesGridModel();
            return Json(viewModel);
        }

        [HttpGet]
        [Route("permissions/tree", Name = ControllerActionRouteNames.Admin.RolesPermissions.PermissionsTree)]
        public async Task<ActionResult> PermissionsTree()
        {
            var viewModel = await Model.GetPermissionsTreeModel();
            return Json(viewModel);
        }

        [HttpGet]
        [Route("permissions/get-by-role", Name = ControllerActionRouteNames.Admin.RolesPermissions.PermissionsGetByRole)]
        public async Task<ActionResult> PermissionsGetByRole(int? RoleID)
        {
            var viewModel = await Model.GetRolePermissions(RoleID);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("save", Name = ControllerActionRouteNames.Admin.RolesPermissions.Save)]
        public async Task<ActionResult> Save(RolePermissionsModel.PageViewModel.RolePermissionSaveSubmitModel submitModel)
        {
            var viewModel = await Model.SaveRolePermissions(submitModel);
            return Json(viewModel);
        }
        #endregion
    }
}