using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
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
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, UserID: key, SubmitModelJson: values);
            return Ok();
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.UserManagement.UsersGridUpdate)]        
        public async Task<ActionResult> UsersGridUpdate(int? key, string values)
        {
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, UserID: key, SubmitModelJson: values);
            return Ok();
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.UserManagement.UsersGridDelete)]
        public async Task<ActionResult> UsersGridDelete(int? key)
        {
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.DELETE, UserID: key);
            return Ok();
        }
    }
}