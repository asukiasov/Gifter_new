using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/error-log")]
    public class ErrorLogControllers :  AdminControllerBase<ErrorLogModel>
    {
        [Route("", Name = ControllerActionRouteNames.Admin.ErrorLogController.ErrorLog)]
        public IActionResult ErrorLogs()
        {
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.ErrorLog.ErrorLogView, viewModel);
        }

        [HttpPost]
        [Route("clear", Name = ControllerActionRouteNames.Admin.ErrorLogController.Clear)]
        public IActionResult Clear()
        {
            var viewModel = Model.ClearLogs();
            return Json(viewModel);
        }
    }
}