using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/permissions")]
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
        [Route("", Name = ControllerActionRouteNames.Admin.Permissions.Page)]
        public ActionResult Permissions()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.Permissions.Page, viewModel);
        }

        [Route("tree", Name = ControllerActionRouteNames.Admin.Permissions.Tree)]
        public async Task<ActionResult> PermissionsTree()
        {
            var viewModel = await Model.GetGridViewModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("tree/add", Name = ControllerActionRouteNames.Admin.Permissions.TreeAdd)]
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
        [Route("tree/update", Name = ControllerActionRouteNames.Admin.Permissions.TreeUpdate)]
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
        [Route("tree/delete", Name = ControllerActionRouteNames.Admin.Permissions.TreeDelete)]
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
}