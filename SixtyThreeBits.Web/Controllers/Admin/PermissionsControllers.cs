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
        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.PermissionsController.Permissions)]
        public ActionResult Permissions()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.Permissions.PermissionsView, viewModel);
        }

        [Route("tree", Name = ControllerActionRouteNames.Admin.PermissionsController.Tree)]
        public async Task<ActionResult> Tree()
        {
            var viewModel = await Model.ListTreeItems();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("tree/add", Name = ControllerActionRouteNames.Admin.PermissionsController.TreeAdd)]
        public async Task<ActionResult> TreeAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<PermissionsModel.ViewModel.TreeViewModel.TreeItem>() ?? new PermissionsModel.ViewModel.TreeViewModel.TreeItem();
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
        [Route("tree/update", Name = ControllerActionRouteNames.Admin.PermissionsController.TreeUpdate)]
        public async Task<ActionResult> TreeUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<PermissionsModel.ViewModel.TreeViewModel.TreeItem>() ?? new PermissionsModel.ViewModel.TreeViewModel.TreeItem();
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
        [Route("tree/delete", Name = ControllerActionRouteNames.Admin.PermissionsController.TreeDelete)]
        public async Task<ActionResult> TreeDelete(int? key)
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