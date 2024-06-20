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
    public class RolesControllers : AdminControllerBase<RolesModel>
    {
        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.RolesControllers.Roles)]
        public ActionResult Roles()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.Roles.RolesView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.RolesControllers.Grid)]
        public async Task<ActionResult> Grid()
        {
            var viewModel = await Model.ListGridItem();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.RolesControllers.GridAdd)]
        public async Task<ActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<RolesModel.ViewModel.GridViewModel.GridItem>() ?? new RolesModel.ViewModel.GridViewModel.GridItem();
            await Model.IUD(databaseAction: Enums.DatabaseActions.CREATE, roleID: key, submitModel: submitModel);
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
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.RolesControllers.GridUpdate)]
        public async Task<ActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<RolesModel.ViewModel.GridViewModel.GridItem>() ?? new RolesModel.ViewModel.GridViewModel.GridItem();
            await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, roleID: key, submitModel: submitModel);
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
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.RolesControllers.GridDelete)]
        public async Task<ActionResult> GridDelete(int? key)
        {
            await Model.IUD(databaseAction: Enums.DatabaseActions.DELETE, roleID: key, submitModel: new RolesModel.ViewModel.GridViewModel.GridItem());
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