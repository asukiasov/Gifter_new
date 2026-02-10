using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class ScraperController : WebsiteControllerBase<ScraperModel>
    {
        #region Nested Classes
        public class ScrapeRequest
        {
            public string Url { get; set; }
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("scraper", Name = ControllerActionRouteNames.Website.ScraperController.Test)]
        [Route($"{RouteValueLanguageCode}/scraper", Name = ControllerActionRouteNames.Website.ScraperController.Test + "-culture")]
        public IActionResult Test()
        {
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Website.Scraper.TestView, viewModel);
        }

        [HttpPost]
        [Route("scraper/api/scrape", Name = ControllerActionRouteNames.Website.ScraperController.Scrape)]
        public async Task<IActionResult> Scrape([FromBody] ScrapeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Url))
            {
                return BadRequest(new { success = false, error = "URL is required" });
            }

            var result = await Model.ScrapeAsync(request.Url);
            return Ok(result);
        }
        #endregion
    }
}
