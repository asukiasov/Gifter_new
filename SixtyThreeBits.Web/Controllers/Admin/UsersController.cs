using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/users")]
    public class UsersController : AdminControllerBase<UsersModel>
    {
        #region Constructors
        public UsersController()
        {
            Model = new UsersModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Users.Page)]
        public async Task<ActionResult> Users()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.Users.Page, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Users.Grid)]
        public async Task<ActionResult> UsersGrid()
        {
            var viewModel = await Model.GetGridViewModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Users.GridAdd)]
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
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.Users.GridUpdate)]
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
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.Users.GridDelete)]
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
}