using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Controllers
{
    [Route("admin/um/users")]
    public class UsersController : AdminControllerBase<UsersModel>
    {        
        #region Constructors
        public UsersController()
        {
            Model = new UsersModel();
        }
        #endregion

        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.UserManagement.Users)]
        public async Task<ActionResult> Users()
        {
            Model.PluginClient.EnableDevextreme(true);
            var ViewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.UserManagement.Users, ViewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.UserManagement.UsersGrid)]
        public async Task<ActionResult> UsersGrid()
        {
            var ViewModel = await Model.GetGridViewModel();
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.UserManagement.UsersGridAdd)]
        public async Task<ActionResult> UsersGridAdd(int? key, string values)
        {
            var SubmitModel = values.FromJsonTo<UsersModel.PageViewModel.GridModel.GridItem>() ?? new UsersModel.PageViewModel.GridModel.GridItem();
            await Model.ValidateUserEmail(UserEmail: SubmitModel.UserEmail, UserID: key);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, UserID: key, SubmitModel: SubmitModel);
                return Ok();
            }
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.UserManagement.UsersGridUpdate)]
        public async Task<ActionResult> UsersGridUpdate(int? key, string values)
        {
            var Result = default(ActionResult);
            var SubmitModel = values.FromJsonTo<UsersModel.PageViewModel.GridModel.GridItem>() ?? new UsersModel.PageViewModel.GridModel.GridItem();

            await Model.ValidateUserEmail(UserEmail: SubmitModel.UserEmail, UserID: key);
            if (Model.Form.HasErrors)
            {
                Result = GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, UserID: key, SubmitModel: SubmitModel);
                if (Model.Form.HasErrors)
                {
                    Result = GetDevexpressErrorResult(Model.Form.ErrorMessage);
                }
                else
                {
                    Result = Ok();
                }
            }

            return Result;
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.UserManagement.UsersGridDelete)]
        public async Task<ActionResult> UsersGridDelete(int? key)
        {
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.DELETE, UserID: key, SubmitModel: new UsersModel.PageViewModel.GridModel.GridItem());
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return Ok();
            }
        } 
        #endregion
    }

    [Route("admin/um/roles")]
    public class RolesController : AdminControllerBase<RolesModel>
    {
        #region Constructors
        public RolesController()
        {
            Model = new RolesModel();
        }
        #endregion

        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.UserManagement.Roles)]
        public ActionResult Roles()
        {
            Model.PluginClient.EnableDevextreme(true);
            var ViewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.UserManagement.Roles, ViewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.UserManagement.RolesGrid)]
        public async Task<ActionResult> RolesGrid()
        {
            var ViewModel = await Model.GetGridViewModel();
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.UserManagement.RolesGridAdd)]
        public async Task<ActionResult> RolesGridAdd(int? key, string values)
        {
            var SubmitModel = values.FromJsonTo<RolesModel.PageViewModel.GridModel.GridItem>() ?? new RolesModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, RoleID: key, SubmitModel: SubmitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return Ok();
            }
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.UserManagement.RolesGridUpdate)]
        public async Task<ActionResult> RolesGridUpdate(int? key, string values)
        {
            var SubmitModel = values.FromJsonTo<RolesModel.PageViewModel.GridModel.GridItem>() ?? new RolesModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, RoleID: key, SubmitModel: SubmitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return Ok();
            }
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.UserManagement.RolesGridDelete)]
        public async Task<ActionResult> RolesGridDelete(int? key)
        {
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.DELETE, RoleID: key, SubmitModel: new RolesModel.PageViewModel.GridModel.GridItem());
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return Ok();
            }
        } 
        #endregion
    }

    [Route("admin/um/permissions")]
    public class PermissionsController : AdminControllerBase<PermissionsModel>
    {
        #region Constructors
        public PermissionsController()
        {
            Model = new PermissionsModel();
        }
        #endregion

        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.UserManagement.Permissions)]
        public ActionResult Permissions()
        {
            Model.PluginClient.EnableDevextreme(true);
            var ViewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.UserManagement.Permissions, ViewModel);
        }

        [Route("tree", Name = ControllerActionRouteNames.Admin.UserManagement.PermissionsTree)]
        public async Task<ActionResult> PermissionsTree()
        {
            var ViewModel = await Model.GetGridViewModel();
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("tree/add", Name = ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeAdd)]
        public async Task<ActionResult> PermissionsTreeAdd(int? key, string values)
        {
            var SubmitModel = values.FromJsonTo<PermissionsModel.PageViewModel.TreeModel.TreeItem>() ?? new PermissionsModel.PageViewModel.TreeModel.TreeItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, PermissionID: key, SubmitModel: SubmitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return Ok();
            }
        }

        [HttpPut]
        [Route("tree/update", Name = ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeUpdate)]
        public async Task<ActionResult> PermissionsTreeUpdate(int? key, string values)
        {
            var SubmitModel = values.FromJsonTo<PermissionsModel.PageViewModel.TreeModel.TreeItem>() ?? new PermissionsModel.PageViewModel.TreeModel.TreeItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, PermissionID: key, SubmitModel: SubmitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return Ok();
            }
        }

        [HttpDelete]
        [Route("tree/delete", Name = ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeDelete)]
        public async Task<ActionResult> PermissionsTreeDelete(int? key)
        {
            await Model.DeleteRecursive(PermissionID: key);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return Ok();
            }
        } 
        #endregion
    }

    [Route("admin/um/role-permissions")]
    public class RolePermissionsController: AdminControllerBase<RolePermissionsModel>
    {
        #region Constructors
        public RolePermissionsController()
        {            
            Model = new RolePermissionsModel();            
        }
        #endregion

        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.UserManagement.RolePermissions)]
        public ActionResult RolePermissions()
        {
            Model.PluginClient.EnableDevextreme(true).EnableSuccessErrorMessage(true);
            Model.SuccessErrorPartialViewModel.IsInitialized = true;
            var ViewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.UserManagement.RolePermissions, ViewModel);
        }

        [HttpGet]
        [Route("get", Name = ControllerActionRouteNames.Admin.UserManagement.RolePermissionsGet)]
        public async Task<ActionResult> RolePermissionsGet(int? RoleID)
        {
            var ViewModel = await Model.GetRolePermissions(RoleID);
            return Json(ViewModel);
        }

        [HttpGet]
        [Route("roles/grid", Name = ControllerActionRouteNames.Admin.UserManagement.RolePermissionsRolesGrid)]
        public async Task<ActionResult> RolePermissionsRolesGrid()
        {
            var ViewModel = await Model.GetRolesGridModel();
            return Json(ViewModel);
        }

        [HttpGet]
        [Route("permissions/tree", Name = ControllerActionRouteNames.Admin.UserManagement.RolePermissionsPermissionsTree)]
        public async Task<ActionResult> RolePermissionsPermissionsTree()
        {
            var ViewModel = await Model.GetPermissionsTreeModel();
            return Json(ViewModel);
        }

        [HttpPut]
        [Route("save", Name = ControllerActionRouteNames.Admin.UserManagement.RolePermissionsSave)]
        public async Task<ActionResult> RolePermissionsSave(RolePermissionsModel.PageViewModel.RolePermissionSaveSubmitModel SubmitModel)
        {
            var ViewModel = await Model.SaveRolePermissions(SubmitModel);
            return Json(ViewModel);
        } 
        #endregion
    }
}