using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/users/{userID:int}/followers")]
    [TypeFilter(typeof(UserFilterAttribute), Order = 2)]
    public class UserFollowersController : AdminControllerBase<UserFollowersModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.UserFollowersController.Followers)]
        public async Task<IActionResult> Followers()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = await Model.GetViewModel();
            return View(ViewNames.Admin.Users.User.UserFollowersView, viewModel);
        }

        [HttpGet]
        [Route("grid", Name = ControllerActionRouteNames.Admin.UserFollowersController.FollowersGrid)]
        public async Task<IActionResult> FollowersGrid()
        {
            var viewModel = await Model.GetGridItems();
            return DevExtremeGridResult(viewModel);
        }
        #endregion
    }
}
