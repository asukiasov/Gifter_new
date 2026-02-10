using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class WishlistController : WebsiteControllerBase<WishlistDetailModel>
    {
        #region Nested Classes
        public class ScrapeRequest
        {
            public string Url { get; set; }
        }
        #endregion

        #region Actions
        [Route("wishlist/{id:int}", Name = ControllerActionRouteNames.Website.WishlistController.Detail)]
        public async Task<IActionResult> Detail(int id)
        {
            await Model.LoadAsync(id);

            if (Model.NotFound)
            {
                return NotFound();
            }

            return View(ViewNames.Website.Wishlist.DetailView, Model);
        }

        [HttpPost]
        [Route("api/wishlist/{id:int}/scrape", Name = ControllerActionRouteNames.Website.WishlistController.Scrape)]
        public async Task<IActionResult> Scrape(int id, [FromBody] ScrapeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Url))
            {
                return BadRequest(new { success = false, error = "URL is required" });
            }

            var result = await Model.ScrapeProductAsync(request.Url);
            return Ok(result);
        }

        [HttpPost]
        [Route("api/wishlist/{id:int}/add-product", Name = ControllerActionRouteNames.Website.WishlistController.AddProduct)]
        public async Task<IActionResult> AddProduct(int id, [FromBody] WishlistDetailModel.AddProductRequest request)
        {
            if (Model.User == null)
            {
                return Unauthorized(new { success = false, error = "You must be logged in to add products" });
            }

            var result = await Model.AddProductAsync(id, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        #endregion
    }
}
