using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/roles")]
    public class RolesControllers : AdminControllerBase<RolesModel>
    {
        #region Constructors
        public RolesControllers()
        {
            Model = new RolesModel();
        }
        #endregion

        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Roles.Page)]
        public ActionResult Roles()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.Roles.Page, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Roles.Grid)]
        public async Task<ActionResult> RolesGrid()
        {
            var viewModel = await Model.GetGridViewModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Roles.GridAdd)]
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
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.Roles.GridUpdate)]
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
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.Roles.GridDelete)]
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
}