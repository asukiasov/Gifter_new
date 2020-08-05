using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Admin.Filters;
using SixtyThreeBits.Web.Reusables.Core;

namespace SixtyThreeBits.Web.Admin.Controllers
{    
    [TypeFilter(typeof(BeforeAdminPageLoad), Order = 1)]
    public class AdminControllerBase<T> : WebProjectController<T>
    {
        [NonAction]
        public ContentResult GetDevexpressErrorResult(string ErrorMessage)
        {
            return new ContentResult { Content = ErrorMessage, StatusCode = 500 };
        }

        [NonAction]
        public JsonResult GetDevexpressSuccessResult()
        {
            return Json("OK");
        }                
    }
}
