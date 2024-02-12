using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Admin.Filters;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Domain;
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
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.UserManagement.Users, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.UserManagement.UsersGrid)]
        public async Task<ActionResult> UsersGrid()
        {
            var viewModel = await Model.GetGridViewModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.UserManagement.UsersGridAdd)]
        public async Task<ActionResult> UsersGridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<UsersModel.PageViewModel.GridModel.GridItem>() ?? new UsersModel.PageViewModel.GridModel.GridItem();
            await Model.ValidateUserEmail(userEmail: submitModel.UserEmail, userID: key);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                await Model.CRUD(databaseAction: Enums.DatabaseActions.CREATE, userID: key, submitModel: submitModel);
                return GetDevexpressSuccessResult();
            }
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.UserManagement.UsersGridUpdate)]
        public async Task<ActionResult> UsersGridUpdate(int? key, string values)
        {
            var result = default(ActionResult);
            var submitModel = values.DeserializeJsonTo<UsersModel.PageViewModel.GridModel.GridItem>() ?? new UsersModel.PageViewModel.GridModel.GridItem();

            await Model.ValidateUserEmail(userEmail: submitModel.UserEmail, userID: key);
            if (Model.Form.HasErrors)
            {
                result = GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                await Model.CRUD(databaseAction: Enums.DatabaseActions.UPDATE, userID: key, submitModel: submitModel);
                if (Model.Form.HasErrors)
                {
                    result = GetDevexpressErrorResult(Model.Form.ErrorMessage);
                }
                else
                {
                    result = GetDevexpressSuccessResult();
                }
            }

            return result;
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.UserManagement.UsersGridDelete)]
        public async Task<ActionResult> UsersGridDelete(int? key)
        {
            await Model.CRUD(databaseAction: Enums.DatabaseActions.DELETE, userID: key, submitModel: new UsersModel.PageViewModel.GridModel.GridItem());
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
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
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.UserManagement.Roles, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.UserManagement.RolesGrid)]
        public async Task<ActionResult> RolesGrid()
        {
            var viewModel = await Model.GetGridViewModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.UserManagement.RolesGridAdd)]
        public async Task<ActionResult> RolesGridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<RolesModel.PageViewModel.GridModel.GridItem>() ?? new RolesModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.CREATE, roleID: key, submitModel: submitModel);
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
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.UserManagement.RolesGridUpdate)]
        public async Task<ActionResult> RolesGridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<RolesModel.PageViewModel.GridModel.GridItem>() ?? new RolesModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.UPDATE, roleID: key, submitModel: submitModel);
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
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.UserManagement.RolesGridDelete)]
        public async Task<ActionResult> RolesGridDelete(int? key)
        {
            await Model.CRUD(databaseAction: Enums.DatabaseActions.DELETE, roleID: key, submitModel: new RolesModel.PageViewModel.GridModel.GridItem());
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
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
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.UserManagement.Permissions, viewModel);
        }

        [Route("tree", Name = ControllerActionRouteNames.Admin.UserManagement.PermissionsTree)]
        public async Task<ActionResult> PermissionsTree()
        {
            var viewModel = await Model.GetGridViewModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("tree/add", Name = ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeAdd)]
        public async Task<ActionResult> PermissionsTreeAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<PermissionsModel.PageViewModel.TreeModel.TreeItem>() ?? new PermissionsModel.PageViewModel.TreeModel.TreeItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.CREATE, permissionID: key, submitModel: submitModel);
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
        [Route("tree/update", Name = ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeUpdate)]
        public async Task<ActionResult> PermissionsTreeUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<PermissionsModel.PageViewModel.TreeModel.TreeItem>() ?? new PermissionsModel.PageViewModel.TreeModel.TreeItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.UPDATE, permissionID: key, submitModel: submitModel);
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
        [Route("tree/delete", Name = ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeDelete)]
        public async Task<ActionResult> PermissionsTreeDelete(int? key)
        {
            await Model.DeleteRecursive(permissionID: key);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
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
            Model.PluginsClient.EnableDevextreme(true).EnableSuccessErrorMessage(true);
            Model.SuccessErrorPartialViewModel.IsInitialized = true;
            var viewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.UserManagement.RolePermissions, viewModel);
        }

        [HttpGet]
        [Route("get", Name = ControllerActionRouteNames.Admin.UserManagement.RolePermissionsGet)]
        public async Task<ActionResult> RolePermissionsGet(int? RoleID)
        {
            var viewModel = await Model.GetRolePermissions(RoleID);
            return Json(viewModel);
        }

        [HttpGet]
        [Route("roles/grid", Name = ControllerActionRouteNames.Admin.UserManagement.RolePermissionsRolesGrid)]
        public async Task<ActionResult> RolePermissionsRolesGrid()
        {
            var viewModel = await Model.GetRolesGridModel();
            return Json(viewModel);
        }

        [HttpGet]
        [Route("permissions/tree", Name = ControllerActionRouteNames.Admin.UserManagement.RolePermissionsPermissionsTree)]
        public async Task<ActionResult> RolePermissionsPermissionsTree()
        {
            var viewModel = await Model.GetPermissionsTreeModel();
            return Json(viewModel);
        }

        [HttpPut]
        [Route("save", Name = ControllerActionRouteNames.Admin.UserManagement.RolePermissionsSave)]
        public async Task<ActionResult> RolePermissionsSave(RolePermissionsModel.PageViewModel.RolePermissionSaveSubmitModel submitModel)
        {
            var viewModel = await Model.SaveRolePermissions(submitModel);
            return Json(viewModel);
        }
        #endregion
    }

    [Route("admin/um/users/{userID:int}/properties")]
    [TypeFilter(typeof(BeforeUserPageLoad), Order = 2)]
    public class UserPropertiesController : AdminControllerBase<UserPropertiesModel>
    {
        #region Constructors
        public UserPropertiesController()
        {
            Model = new UserPropertiesModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.UserManagement.User.Properties)]
        public async Task<IActionResult> UserProperties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableDevextreme(true).EnableJQueryMaskedInput(true);
            var viewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.UserManagement.User.Properties, viewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> UserPropertiesSave(UserPropertiesModel.PageViewModel submitModel)
        {
            var Result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableDevextreme(true).EnableJQueryMaskedInput(true);
            var viewModel = await Model.GetPageViewModel(submitModel);

            await Model.ValidatePageViewModel(viewModel);
            if (viewModel.IsValid)
            {
                await Model.Save(viewModel);
                if (viewModel.IsSaved)
                {
                    Result = Redirect(Model.UrlCurrentPageWithDomain);
                    Model.ShowSuccess();
                }
                else
                {
                    Result = View(ViewNames.Admin.UserManagement.User.Properties, viewModel);
                    Model.ShowError(viewModel.ErrorMessage);
                }
            }
            else
            {
                Result = View(ViewNames.Admin.UserManagement.User.Properties, viewModel);
            }

            return Result;
        }
        #endregion
    }
}