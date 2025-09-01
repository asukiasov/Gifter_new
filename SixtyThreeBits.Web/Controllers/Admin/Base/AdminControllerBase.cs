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
        public IActionResult DevExtremeGridResult(AjaxResponse viewModel)
        {
            if (viewModel.IsSuccess)
            {
                return Json(viewModel.Data);
            }
            else
            {
                throw new System.Exception(viewModel.Data.ToString());
            }
        }

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
