using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Base;
using SixtyThreeBits.Web.Filters.Admin;

namespace SixtyThreeBits.Web.Controllers.Admin.Base
{
    [TypeFilter(typeof(BeforeAdminPageLoad), Order = 1)]
    public class AdminControllerBase<T> : ControllerBase<T> where T : new()
    {
        [NonAction]
        public ContentResult GetDevexpressErrorResult(string errorMessage)
        {
            return new ContentResult { Content = errorMessage, StatusCode = 500 };
        }

        [NonAction]
        public JsonResult GetDevexpressSuccessResult()
        {
            return Json("OK");
        }
    }
}
