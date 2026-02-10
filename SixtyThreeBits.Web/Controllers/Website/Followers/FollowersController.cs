using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class FollowersController : WebsiteControllerBase<FollowersModel>
    {
        #region Actions
        [HttpPost]
        [Route("followers/follow", Name = ControllerActionRouteNames.Website.FollowersController.Follow)]
        public async Task<IActionResult> Follow(int followedUserID)
        {
            var viewModel = await Model.Follow(followedUserID);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("followers/unfollow", Name = ControllerActionRouteNames.Website.FollowersController.Unfollow)]
        public async Task<IActionResult> Unfollow(int followedUserID)
        {
            var viewModel = await Model.Unfollow(followedUserID);
            return Json(viewModel);
        }

        [HttpGet]
        [Route("followers/is-following/{followedUserID}", Name = ControllerActionRouteNames.Website.FollowersController.IsFollowing)]
        public async Task<IActionResult> IsFollowing(int followedUserID)
        {
            var viewModel = await Model.IsFollowing(followedUserID);
            return Json(viewModel);
        }

        [HttpGet]
        [Route("followers/following", Name = ControllerActionRouteNames.Website.FollowersController.Following)]
        public async Task<IActionResult> Following()
        {
            var viewModel = await Model.GetFollowing();
            return Json(viewModel);
        }

        [HttpGet]
        [Route("followers/my-followers", Name = ControllerActionRouteNames.Website.FollowersController.MyFollowers)]
        public async Task<IActionResult> MyFollowers()
        {
            var viewModel = await Model.GetMyFollowers();
            return Json(viewModel);
        }
        #endregion
    }
}
