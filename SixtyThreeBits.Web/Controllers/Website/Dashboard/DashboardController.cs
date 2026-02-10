using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class DashboardController : WebsiteControllerBase<DashboardModel>
    {
        #region Actions
        [Route("dashboard", Name = ControllerActionRouteNames.Website.DashboardController.Index)]
        public async Task<IActionResult> Index()
        {
            if (Model.User == null)
            {
                return Redirect("/");
            }

            await Model.LoadWishlistsAsync();
            return View(ViewNames.Website.Dashboard.IndexView, Model);
        }

        [HttpPost]
        [Route("api/users/complete-onboarding")]
        public async Task<IActionResult> CompleteOnboarding()
        {
            if (Model.User == null)
            {
                return Unauthorized(new { success = false, message = "Not authenticated" });
            }

            var result = await Model.CompleteOnboardingAsync();

            if (result)
            {
                return Ok(new { success = true });
            }

            return BadRequest(new { success = false, message = "Failed to complete onboarding" });
        }

        [HttpPost]
        [Route("api/dashboard/create-wishlist")]
        public async Task<IActionResult> CreateWishlist([FromBody] CreateWishlistRequest request)
        {
            if (Model.User == null)
            {
                return Unauthorized(new { success = false, message = "Not authenticated" });
            }

            if (string.IsNullOrWhiteSpace(request?.Title))
            {
                return BadRequest(new { success = false, message = "Wishlist title is required" });
            }

            var giftListID = await Model.CreateWishlistAsync(request.Title, request.OccasionType);

            if (giftListID.HasValue)
            {
                return Ok(new { success = true, giftListID = giftListID.Value });
            }

            return BadRequest(new { success = false, message = "Failed to create wishlist" });
        }
        #endregion
    }

    public class CreateWishlistRequest
    {
        public string Title { get; set; }
        public string OccasionType { get; set; }
    }
}
