using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Controllers.Base;
using SixtyThreeBits.Web.Filters.Admin;

namespace SixtyThreeBits.Web.Controllers.Admin.Base
{
    [TypeFilter(typeof(AdminFilterAttribute), Order = 1)]
    public class AdminControllerBase<T> : ControllerBase<T> where T : new()
    {
        #region Methods
        [NonAction]
        public IActionResult DevExtremeGridActionResult(AjaxResponse viewModel)
        {
            if (viewModel.IsSuccess)
            {
                return Json("OK");
            }
            else
            {
                return new ContentResult { Content = viewModel.Data.ToString(), StatusCode = 500 };
            }
        }        
        #endregion
    }
}
