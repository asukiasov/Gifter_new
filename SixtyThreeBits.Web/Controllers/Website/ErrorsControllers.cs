using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;

namespace SixtyThreeBits.Web.Controllers.Website
{
    [Route("error")]
    public class ErrorsController : WebsiteControllerBase<ErrorsModel>
    {
        #region Actions
        [Route("404")]
        public IActionResult Error404NotFound()
        {
            return View(ViewNames.Website.Errors.NotFoundView);
        } 
        #endregion
    }
}
