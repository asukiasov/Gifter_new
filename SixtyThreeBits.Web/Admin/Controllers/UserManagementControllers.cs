using Microsoft.AspNetCore.Mvc;
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
    }
}